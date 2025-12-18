using System;
using System.Collections.Generic;

namespace Market.API.Models;

public partial class Order
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public DateTime Date { get; set; }

    public decimal? TotalAmount { get; set; }

    public int? StatusId { get; set; }

    public virtual ICollection<OrderGame> OrderGames { get; set; } = new List<OrderGame>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual OrderStatus? Status { get; set; }

    public virtual User? User { get; set; }
}
