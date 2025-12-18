using System;
using System.Collections.Generic;

namespace Market.API.Models;

public partial class GameCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? ParentCategoryId { get; set; }

    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();

    public virtual ICollection<GameCategory> InverseParentCategory { get; set; } = new List<GameCategory>();

    public virtual GameCategory? ParentCategory { get; set; }
}
