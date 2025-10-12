using Market.Core.Entities.Base;

namespace Market.Core.Entities;

public class ProductCategoryEntity : BaseEntity
{
    public string Name { get; set; }
    public bool IsEnabled { get; set; }
}