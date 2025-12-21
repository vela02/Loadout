using System;
using System.Collections.Generic;

namespace Market.Domain.Models;

public partial class Discount
{
    public int Id { get; set; }

    public int? GameId { get; set; }

    public int? CategoryId { get; set; }

    public decimal DiscountPercentage { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string? Description { get; set; }

    public virtual GameCategory? Category { get; set; }

    public virtual Game? Game { get; set; }
}
