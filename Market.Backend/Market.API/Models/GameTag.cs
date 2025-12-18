using System;
using System.Collections.Generic;

namespace Market.API.Models;

public partial class GameTag
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
