using System;
using System.Collections.Generic;

namespace Market.Domain.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public DateTime PaymentDate { get; set; }

    public decimal? Amount { get; set; }

    public string? PaymentMethod { get; set; }

    public int? StatusId { get; set; }

    public virtual Order? Order { get; set; }

    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();

    public virtual PaymentStatus? Status { get; set; }
}
