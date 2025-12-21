using System;
using System.Collections.Generic;

namespace Market.Domain.Models;

public partial class License
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? GameId { get; set; }

    public string LicenseKey { get; set; } = null!;

    public DateTime IssueDate { get; set; }

    public DateTime ExpiryDate { get; set; }

    public virtual Game? Game { get; set; }

    public virtual User? User { get; set; }
}
