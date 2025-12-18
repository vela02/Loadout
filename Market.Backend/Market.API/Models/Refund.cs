using System;
using System.Collections.Generic;

namespace Market.API.Models;

public partial class Refund
{
    public int Id { get; set; }

    public int? PaymentId { get; set; }

    public DateTime RefundDate { get; set; }

    public decimal? Amount { get; set; }

    public string? Reason { get; set; }

    public int? StatusId { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual RefundStatus? Status { get; set; }
}
