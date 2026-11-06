using Market.Application;
using Market.Application.Abstractions;
using Market.Application.Common.Exceptions;
using Market.Domain.Models;
using Market.Shared.Dtos;
using Market.Shared.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Market.Infrastructure.Services;

/// <summary>
/// Stripe naplata (PaymentIntent tok):
///  1) CreatePaymentIntentAsync  – server izračuna iznos iz korpe i kod Stripe-a otvori "namjeru plaćanja"
///  2) Frontend (Stripe.js) pošalje podatke kartice DIREKTNO Stripe-u i potvrdi plaćanje
///     -> broj kartice nikad ne prolazi kroz naš server (PCI sigurnost)
///  3) ConfirmPaymentAsync – server pita Stripe da li je plaćanje stvarno uspjelo,
///     tek onda kreira narudžbe i spremi transakciju u tabelu Payment.
/// </summary>
public class PaymentService : IPaymentService
{
    private readonly LoadoutDbContext _context;
    private readonly IOrderService _orderService;
    private readonly StripeOptions _options;
    private readonly Stripe.PaymentIntentService _intents;

    public PaymentService(LoadoutDbContext context, IOrderService orderService, IOptions<StripeOptions> options)
    {
        _context = context;
        _orderService = orderService;
        _options = options.Value;
        _intents = new Stripe.PaymentIntentService(new Stripe.StripeClient(_options.SecretKey));
    }

    public string GetPublishableKey() => _options.PublishableKey;

    public async Task<PaymentIntentDto> CreatePaymentIntentAsync(int userId)
    {
        EnsureConfigured();

        // Iznos NIKAD ne uzimamo s frontenda – računamo ga iz korpe u bazi (s popustima)
        var total = await _orderService.CalculateCartTotalAsync(userId);
        if (total <= 0)
            throw new MarketBusinessRuleException("cart.empty", "Korpa je prazna.");

        var intent = await _intents.CreateAsync(new Stripe.PaymentIntentCreateOptions
        {
            Amount = ToMinorUnits(total),                  // Stripe radi u najmanjoj jedinici (feninzi)
            Currency = _options.Currency,
            // Stripe.net 53+: umjesto PaymentMethodTypes koristi se AutomaticPaymentMethods.
            // AllowRedirects = "never" -> samo metode bez preusmjeravanja (kartica), što odgovara našem formularu.
            AutomaticPaymentMethods = new Stripe.PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true,
                AllowRedirects = "never",
            },
            Description = $"Loadout narudžba (korisnik #{userId})",
            Metadata = new Dictionary<string, string> { ["userId"] = userId.ToString() },
        });

        return new PaymentIntentDto
        {
            ClientSecret = intent.ClientSecret,
            PaymentIntentId = intent.Id,
            Amount = total,
            Currency = _options.Currency,
        };
    }

    public async Task<CheckoutResultDto> ConfirmPaymentAsync(int userId, string paymentIntentId)
    {
        EnsureConfigured();

        // 1) Ista transakcija se ne smije obraditi dvaput (npr. dupli klik / refresh)
        if (await _context.Payments.AnyAsync(p => p.TransactionId == paymentIntentId))
            throw new MarketConflictException("Ovo plaćanje je već obrađeno.");

        // 2) Pitamo Stripe za stvarno stanje plaćanja (ne vjerujemo frontendu)
        var intent = await _intents.GetAsync(paymentIntentId);

        if (intent.Status != "succeeded")
            throw new MarketBusinessRuleException("payment.not_succeeded", $"Plaćanje nije uspjelo (status: {intent.Status}).");

        if (!intent.Metadata.TryGetValue("userId", out var owner) || owner != userId.ToString())
            throw new MarketBusinessRuleException("payment.wrong_user", "Plaćanje ne pripada ovom korisniku.");

        // 3) Iznos koji je Stripe naplatio mora odgovarati korpi (zaštita od izmjene korpe u međuvremenu)
        var total = await _orderService.CalculateCartTotalAsync(userId);
        if (intent.Amount != ToMinorUnits(total))
            throw new MarketBusinessRuleException("payment.amount_mismatch", "Iznos plaćanja ne odgovara korpi.");

        // 4) Kreiraj narudžbe (plaćena + eventualno pre-order) – postojeća logika checkouta
        var result = await _orderService.CheckoutAsync(userId);
        if (result.OrderId == 0)
            throw new MarketBusinessRuleException("cart.empty", "Korpa je prazna.");

        // 5) Spremi transakciju u tabelu Payment (po jedan zapis za svaku narudžbu)
        var succeededStatusId = await GetOrCreatePaymentStatusAsync("Succeeded", "Plaćanje uspješno (Stripe)");
        var cardInfo = intent.PaymentMethodId != null ? "Stripe – kartica" : "Stripe";

        var orders = await _context.Orders.Where(o => result.OrderIds.Contains(o.Id)).ToListAsync();
        foreach (var order in orders)
        {
            _context.Payments.Add(new Payment
            {
                OrderId = order.Id,
                PaymentDate = DateTime.Now,
                Amount = order.TotalAmount,
                PaymentMethod = cardInfo,
                StatusId = succeededStatusId,
                TransactionId = intent.Id,   // veza na transakciju u Stripe dashboardu
            });
        }
        await _context.SaveChangesAsync();

        result.Message = "Plaćanje uspješno! " + result.Message;
        return result;
    }

    private async Task<int> GetOrCreatePaymentStatusAsync(string name, string description)
    {
        var status = await _context.PaymentStatuses.FirstOrDefaultAsync(s => s.Name == name);
        if (status == null)
        {
            status = new PaymentStatus { Name = name, Description = description };
            _context.PaymentStatuses.Add(status);
            await _context.SaveChangesAsync();
        }
        return status.Id;
    }

    private static long ToMinorUnits(decimal amount) => (long)Math.Round(amount * 100m, MidpointRounding.AwayFromZero);

    private void EnsureConfigured()
    {
        if (string.IsNullOrWhiteSpace(_options.SecretKey))
            throw new MarketBusinessRuleException("stripe.not_configured",
                "Stripe nije podešen: dodaj Stripe:SecretKey u User Secrets (Market.API -> Manage User Secrets).");
    }
}
