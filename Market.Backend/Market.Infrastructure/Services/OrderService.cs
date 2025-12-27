using Market.Application;
using Market.Application.Abstractions;
using Market.Domain.Models;
using Market.Infrastructure.Database;
using Market.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Market.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly LoadoutDbContext _context;

    public OrderService(LoadoutDbContext context)
    {
        _context = context;
    }

    // 1. DODAVANJE U KORPU
    public async Task<bool> AddToCartAsync(int userId, int gameId, int quantity)
    {
        // 1. Nađi korpu
        var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync(); // Ovdje je pucalo jer UpdatedAt nije bio postavljen
        }

        // 2. Provjeri stavku
        var existingItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.GameId == gameId);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            _context.CartItems.Add(new CartItem
            {
                CartId = cart.Id,
                GameId = gameId,
                Quantity = quantity,
                AddedAt = DateTime.Now // Ovo smo već imali, ali provjeri naziv polja u bazi
            });
        }

        return await _context.SaveChangesAsync() > 0;
    }

    // 2. KUPOVINA (CHECKOUT)
    public async Task<CheckoutResultDto> CheckoutAsync(int userId)
    {
        // 1. Povuci korpu sa svim stavkama i podacima o igrama
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Game)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        // Provjera da li je korpa prazna
        if (cart == null || !cart.CartItems.Any())
        {
            return new CheckoutResultDto { OrderId = 0, Message = "Korpa je prazna.", IsPreOrder = false };
        }

        // 2. Provjera: Da li u korpi imamo igru koja tek treba izaći (Pre-order)?
        var today = DateOnly.FromDateTime(DateTime.Now);
        bool isPreOrder = cart.CartItems.Any(ci => ci.Game != null && ci.Game.ReleaseDate > today);

        // 3. Kreiraj novu Narudžbu
        var order = new Order
        {
            UserId = userId,
            Date = DateTime.Now,
            TotalAmount = cart.CartItems.Sum(ci => (ci.Game?.Price ?? 0) * ci.Quantity),

            // Postavi status 4 (Pre-ordered) ako je pre-order, inače status 1 (Plaćeno)
            StatusId = isPreOrder ? 4 : 1,

            ReferenceNumber = "ORD-" + Guid.NewGuid().ToString().ToUpper().Substring(0, 8),
            OrderedAtUtc = DateTime.UtcNow
        };
        _context.Orders.Add(order);

        // 4. Obradi svaku stavku iz korpe
        foreach (var item in cart.CartItems)
        {
            if (item.GameId == null) continue;

            // Spasi zapis u OrderGame (istorija cijena pri kupovini)
            var orderGame = new OrderGame
            {
                Order = order,
                GameId = item.GameId.Value,
                Quantity = item.Quantity,
                PriceAtPurchase = item.Game?.Price ?? 0
            };
            _context.OrderGames.Add(orderGame);

            // 5. GENERISANJE KLJUČEVA: Samo ako je igra VEĆ izašla
            if (item.Game != null && item.Game.ReleaseDate <= today)
            {
                for (int i = 0; i < item.Quantity; i++)
                {
                    var license = new License
                    {
                        UserId = userId,
                        GameId = item.GameId.Value,
                        LicenseKey = Guid.NewGuid().ToString().ToUpper().Substring(0, 16),
                        IssueDate = DateTime.Now,
                        ExpiryDate = DateTime.Now.AddYears(10)
                    };
                    _context.Licenses.Add(license);
                }
            }
        }

        // 6. Isprazni korpu nakon uspješne obrade
        _context.CartItems.RemoveRange(cart.CartItems);

        // 7. Spasi sve u bazu
        await _context.SaveChangesAsync();

        // 8. Vrati rezultat sa personalizovanom porukom
        return new CheckoutResultDto
        {
            OrderId = order.Id,
            IsPreOrder = isPreOrder,
            Message = isPreOrder
                ? "Pre-order uspješan! Igra je rezervisana, ključ će biti dostupan na dan izlaska."
                : "Kupovina uspješna! Digitalni ključevi su generisani i dostupni u historiji."
        };
    }
    public async Task<bool> CancelPreOrderAsync(int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.OrderGames)
            .ThenInclude(og => og.Game)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null || order.StatusId != 4) return false; // Može se otkazati samo ako je status Pre-order

        var today = DateOnly.FromDateTime(DateTime.Now);

        // Provjeri da li je ijedna igra već izašla (ako jeste, ne može se više otkazati pre-order)
        bool alreadyReleased = order.OrderGames.Any(og => og.Game.ReleaseDate <= today);
        if (alreadyReleased) return false;

        // Promijeni status u otkazano
        order.StatusId = 5;

        return await _context.SaveChangesAsync() > 0;
    }

    // 3. PREGLED HISTORIJE
    public async Task<List<OrderHistoryDto>> GetOrderHistoryAsync(int userId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderGames).ThenInclude(og => og.Game)
            .Select(o => new OrderHistoryDto
            {
                Id = o.Id,
                Date = o.Date,
                TotalAmount = o.TotalAmount ?? 0,
                Status = o.Status != null ? o.Status.Name : "N/A",
                Games = o.OrderGames.Select(og => new BoughtGameDto
                {
                    Title = og.Game.Title,
                    PriceAtPurchase = og.PriceAtPurchase ?? 0,
                    LicenseKeys = _context.Licenses
                        .Where(l => l.UserId == userId && l.GameId == og.GameId)
                        .Select(l => l.LicenseKey)
                        .ToList()
                }).ToList()
            }).ToListAsync();
    }

    public async Task<List<CartItemDto>> GetCartAsync(int userId)
    {
        return await _context.CartItems
            .Where(ci => ci.Cart.UserId == userId)
            .Select(ci => new CartItemDto
            {
                CartItemId = ci.Id,
                GameId = ci.GameId ?? 0,
                GameTitle = ci.Game.Title,
                Price = ci.Game.Price ?? 0,
                Quantity = ci.Quantity
            }).ToListAsync();
    }
} 