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
            if (existingItem.IsSavedForLater)
                existingItem.IsSavedForLater = false; // vrati iz "za kasnije" u korpu
            else
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
        // 1. Povuci korpu sa svim podacima
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Game)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        // Naplaćujemo samo stavke koje NISU sačuvane za kasnije
        var itemsToBuy = cart?.CartItems.Where(ci => !ci.IsSavedForLater).ToList() ?? new List<CartItem>();

        if (cart == null || !itemsToBuy.Any())
        {
            return new CheckoutResultDto { OrderId = 0, Message = "Korpa je prazna.", IsPreOrder = false };
        }

        // 2. Podijeli stavke: igre koje su izašle (plaćaju se odmah) i pre-order igre.
        //    Svaka grupa ide u SVOJU narudžbu, kako bi se pre-order mogao otkazati
        //    nezavisno od igara koje su već kupljene.
        var today = DateOnly.FromDateTime(DateTime.Now);
        var releasedItems = itemsToBuy.Where(ci => ci.Game != null && !(ci.Game.ReleaseDate > today)).ToList();
        var preOrderItems = itemsToBuy.Where(ci => ci.Game != null && ci.Game.ReleaseDate > today).ToList();

        var createdOrders = new List<Order>();
        decimal grandTotal = 0;

        if (releasedItems.Any())
        {
            var (order, total) = await CreateOrderAsync(userId, releasedItems, statusId: 1, generateLicenses: true);
            createdOrders.Add(order);
            grandTotal += total;
        }

        if (preOrderItems.Any())
        {
            // Pre-order: status 4, ključevi se NE generišu (dobijaju se na dan izlaska)
            var (order, total) = await CreateOrderAsync(userId, preOrderItems, statusId: 4, generateLicenses: false);
            createdOrders.Add(order);
            grandTotal += total;
        }

        // 3. Isprazni korpu (samo kupljene stavke; "za kasnije" ostaju)
        _context.CartItems.RemoveRange(itemsToBuy);

        // 4. Spasi sve promjene u bazu (jedna transakcija za sve narudžbe)
        await _context.SaveChangesAsync();

        bool hasPreOrder = preOrderItems.Any();
        return new CheckoutResultDto
        {
            OrderId = createdOrders.First().Id,
            OrderIds = createdOrders.Select(o => o.Id).ToList(),
            IsPreOrder = hasPreOrder,
            TotalAmount = grandTotal,
            Message = hasPreOrder
                ? "Kupovina uspješna! Pre-order igre su rezervisane u posebnoj narudžbi."
                : "Kupovina uspješna! Status je 'Placeno', popusti su obračunati i ključevi dostupni."
        };
    }

    /// <summary>Ukupan iznos aktivnih stavki korpe (s popustima) – koristi se za Stripe naplatu.</summary>
    public async Task<decimal> CalculateCartTotalAsync(int userId)
    {
        var items = await _context.CartItems
            .Include(ci => ci.Game)
            .Where(ci => ci.Cart!.UserId == userId && !ci.IsSavedForLater && ci.Game != null)
            .ToListAsync();

        decimal total = 0;
        foreach (var item in items)
            total += await GetPriceToPayAsync(item.Game!) * item.Quantity;
        return total;
    }

    /// <summary>Cijena igre s najvećim aktivnim popustom (isto pravilo kao pri kreiranju narudžbe).</summary>
    private async Task<decimal> GetPriceToPayAsync(Game game)
    {
        var activeDiscount = await _context.Discounts
            .Where(d => d.GameId == game.Id && d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now)
            .OrderByDescending(d => d.DiscountPercentage)
            .FirstOrDefaultAsync();

        decimal basePrice = game.Price ?? 0;
        return activeDiscount != null
            ? basePrice - (basePrice * (activeDiscount.DiscountPercentage / 100.0m))
            : basePrice;
    }

    /// <summary>
    /// Kreira jednu narudžbu od zadanih stavki korpe: obračuna popuste, upiše OrderGame
    /// i (opcionalno) generiše licencne ključeve. Ne poziva SaveChanges.
    /// </summary>
    private async Task<(Order order, decimal total)> CreateOrderAsync(
        int userId, List<CartItem> items, int statusId, bool generateLicenses)
    {
        var order = new Order
        {
            UserId = userId,
            Date = DateTime.Now,
            TotalAmount = 0,
            StatusId = statusId,
            ReferenceNumber = "ORD-" + Guid.NewGuid().ToString().ToUpper().Substring(0, 8),
            OrderedAtUtc = DateTime.UtcNow
        };
        _context.Orders.Add(order);

        decimal runningTotal = 0;

        foreach (var item in items)
        {
            if (item.GameId == null || item.Game == null) continue;

            // Aktivan popust (ako se više promocija preklapa, kupac dobija najveći)
            decimal priceToPay = await GetPriceToPayAsync(item.Game);

            runningTotal += priceToPay * item.Quantity;

            _context.OrderGames.Add(new OrderGame
            {
                Order = order,
                GameId = item.GameId.Value,
                Quantity = item.Quantity,
                PriceAtPurchase = priceToPay
            });

            if (generateLicenses)
            {
                for (int i = 0; i < item.Quantity; i++)
                {
                    _context.Licenses.Add(new License
                    {
                        UserId = userId,
                        GameId = item.GameId.Value,
                        LicenseKey = Guid.NewGuid().ToString().ToUpper().Substring(0, 16),
                        IssueDate = DateTime.Now,
                        ExpiryDate = DateTime.Now.AddYears(10)
                    });
                }
            }
        }

        order.TotalAmount = runningTotal;
        return (order, runningTotal);
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
        var today = DateOnly.FromDateTime(DateTime.Now);

        return await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.Status) // Povlači tabelu OrderStatus (onu sa tvoje slike)
            .Include(o => o.OrderGames)
                .ThenInclude(og => og.Game)
            .OrderByDescending(o => o.Date)
            .Select(o => new OrderHistoryDto
            {
                Id = o.Id,
                Date = o.Date,
                TotalAmount = o.TotalAmount ?? 0,

                // Čitamo "Name" direktno iz tvoje tabele statusa
                Status = o.Status != null ? o.Status.Name : "N/A",
                StatusId = o.StatusId,
                ReferenceNumber = o.ReferenceNumber,
                // Ista pravila kao u CancelPreOrderAsync: status 4 (Pre-order) i nijedna igra još nije izašla
                CanCancel = o.StatusId == 4 && o.OrderGames.All(og => og.Game.ReleaseDate > today),

                Games = o.OrderGames.Select(og => new BoughtGameDto
                {
                    GameId = og.GameId,
                    Title = og.Game.Title,
                    ImageUrl = og.Game.ImageUrl,
                    ReleaseDate = og.Game.ReleaseDate,
                    PriceAtPurchase = og.PriceAtPurchase ?? 0,
                    // Povlačimo ključeve iz tabele License
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
                Quantity = ci.Quantity,
                ImageUrl = ci.Game.ImageUrl,
                ReleaseDate = ci.Game.ReleaseDate,
                IsSavedForLater = ci.IsSavedForLater
            }).ToListAsync();
    }

    public async Task<bool> RemoveFromCartAsync(int userId, int cartItemId)
    {
        // Provjera vlasništva: stavka mora pripadati korpi ovog korisnika
        var item = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart!.UserId == userId);
        if (item == null) return false;

        _context.CartItems.Remove(item);
        return await _context.SaveChangesAsync() > 0;
    }

    /// <summary>"Sačuvaj za kasnije" direktno sa stranice igre (bez dodavanja u aktivnu korpu).</summary>
    public async Task<bool> SaveGameForLaterAsync(int userId, int gameId)
    {
        var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null)
        {
            cart = new Cart { UserId = userId, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        var item = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.GameId == gameId);
        if (item == null)
        {
            _context.CartItems.Add(new CartItem
            {
                CartId = cart.Id,
                GameId = gameId,
                Quantity = 1,
                AddedAt = DateTime.Now,
                IsSavedForLater = true
            });
        }
        else
        {
            item.IsSavedForLater = true; // ako je već u korpi, premjesti je u "za kasnije"
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetSavedForLaterAsync(int userId, int cartItemId, bool saved)
    {
        var item = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart!.UserId == userId);
        if (item == null) return false;

        item.IsSavedForLater = saved;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<SalesReportDto> GetSalesReportAsync(DateTime from, DateTime to)
    {
        //date filter
        var ordersQuery = _context.Orders.AsNoTracking()
            .Where(x => x.Date >= from && x.Date <= to);

        var totalOrders = await ordersQuery.CountAsync();

        var totalRevenue = await ordersQuery
       .Where(x => x.TotalAmount != null)
       .SumAsync(x => x.TotalAmount!.Value);

        var totalItemsSold = await _context.OrderGames
            .Where(x => x.Order.Date >= from && x.Order.Date <= to)
            .SumAsync(x => x.Quantity);

        return new SalesReportDto
        {
            TotalOrders=totalOrders,
            TotalRevenue=totalRevenue,
            TotalItemsSold= totalItemsSold
        };
    }
} 