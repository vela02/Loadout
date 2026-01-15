

namespace Market.Application.Modules.Catalog.Discounts.Commands.Create
{
    public class CreateDiscountCommand : IRequest<int>
    {
        
        public int? GameId { get; set; }

        
        public int? CategoryId { get; set; }

        public decimal DiscountPercentage { get; set; } 

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? Description { get; set; }
    }
}
