using System;
using System.Collections.Generic;

namespace Market.API.Models;

public partial class OrderGame
{
    public int OrderId { get; set; }

    public int GameId { get; set; }

    public decimal? PriceAtPurchase { get; set; }

    public int Quantity { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
