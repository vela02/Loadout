using System;
using System.Collections.Generic;

namespace Market.Domain.Models;

public partial class RefundStatus
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();
}
