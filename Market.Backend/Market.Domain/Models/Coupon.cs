using System;
using System.Collections.Generic;

namespace Market.Domain.Models;

public partial class Coupon
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public int? UserId { get; set; }

    public decimal DiscountPercentage { get; set; }

    public DateTime ExpiryDate { get; set; }

    public bool IsActive { get; set; }

    public bool IsEnabled { get; set; }

    public virtual User? User { get; set; }
}
