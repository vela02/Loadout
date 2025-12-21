using Market.Domain.Models;
using System;
using System.Collections.Generic;

namespace Market.Domain.Models;

public partial class Wishlist
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? GameId { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual Game? Game { get; set; }

    public virtual User? User { get; set; }
}
