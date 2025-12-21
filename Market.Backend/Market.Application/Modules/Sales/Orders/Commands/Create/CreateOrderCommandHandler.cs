using Market.Domain.Models;

namespace Market.Application.Modules.Sales.Orders.Commands.Create;

public class CreateOrderCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<CreateOrderCommand, int>
{
    public async Task<int> Handle(CreateOrderCommand request, CancellationToken ct)
    {
    //    #region Create order and set basic properties
    //    var order = new Order
    //    {
            
    //        UserId = currentUser.UserId!.Value,
    //        Date = DateTime.Now,        
    //        TotalAmount = 0m,           
    //        StatusId = 1                
                                        
    //    };
    //    ctx.Orders.Add(order);
    //    #endregion

    //    #region Load products from database and prepare a map

    //    // pokupiti sve id-ove proizvoda koji se naručuju
    //    List<int> productIds = request.Items.Select(ri => ri.ProductId).ToList(); // ne treba hashset jer filter se radi u bazi

    //    List<Game> products = await ctx.Products
    //        .Where(p => productIds.Contains(p.Id)) //<-- dorada nakon nastave za poboljsanje performansi: filtrirati samo proizvode koji su u request.Items
    //        .AsNoTracking()
    //        .ToListAsync(ct);

    //    Dictionary<int, Game> productsMap = products.ToDictionary(x => x.Id);
    //    #endregion

    //    #region Create order items and calculate totals

    //    // za demo svrhe, svi proizvodi imaju 5% popusta
    //    decimal discountPercent = 0.05m;

    //    foreach (var item in request.Items)
    //    {
    //        Game? product = productsMap.GetValueOrDefault(item.ProductId); //<--- bolja performansa O(n) jer koristi dictionary

    //        if (product is null)
    //        {
    //            throw new ValidationException(message: $"Invalid productId {item.ProductId}.");
    //        }

    //        if (product.IsEnabled == false)
    //        {
    //            throw new ValidationException($"Product {product.Title} is disabled.");
    //        }

    //        decimal subtotal = RoundMoney((decimal)(product.Price * item.Quantity));
    //        decimal discountAmount = RoundMoney(subtotal * discountPercent);
    //        decimal total = RoundMoney(subtotal - discountAmount);

    //        var orderGame = new OrderGame
    //        {
    //            Order = order,               
    //            GameId = product.Id,         
    //            Quantity = (int)item.Quantity,    
    //            PriceAtPurchase = product.Price 
    //        };

    //        ctx.OrderGames.Add(orderGame);

           
    //        order.TotalAmount += subtotal;
    //    }
    //    #endregion

    //    await ctx.SaveChangesAsync(ct);

    //    return order.Id;
    //}

    //private static decimal RoundMoney(decimal value)
    //{
    //    return Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}