using System;
using System.Collections.Generic;

namespace Market.Domain.Models;

public partial class Comment
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? GameId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Game? Game { get; set; }

    public virtual User? User { get; set; }
}
