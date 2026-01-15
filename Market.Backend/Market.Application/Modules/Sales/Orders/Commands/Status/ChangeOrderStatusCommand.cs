using Market.Domain.Entities.Sales;

namespace Market.Application.Modules.Sales.Orders.Commands.Status
{
    /// <summary>
    /// Command to change order status
    /// </summary>
    public class ChangeOrderStatusCommand : IRequest
    {
        [JsonIgnore]
        public int Id { get; set; }

        public int NewStatusId { get; set; }
    }
}
