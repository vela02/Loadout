using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Market.Domain.Models;

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


    // to get the build to pass, addIng the following not-mapped properties, later to be removed
    [NotMapped] public string ReferenceNumber { get; set; } = "";
    [NotMapped] public DateTime OrderedAtUtc { get; set; } = DateTime.Now;
    [NotMapped] public string Note { get; set; } = "";
    [NotMapped] public int MarketUserId { get; set; }
}
