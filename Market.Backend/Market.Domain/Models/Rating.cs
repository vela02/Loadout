using System;
using System.Collections.Generic;

namespace Market.Domain.Models;

public partial class Rating
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? GameId { get; set; }

    public int Stars { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Game? Game { get; set; }

    public virtual User? User { get; set; }
}
