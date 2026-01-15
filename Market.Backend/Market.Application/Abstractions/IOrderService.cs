using Market.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Abstractions
{
    public interface IOrderService
    {
        Task<bool> AddToCartAsync(int userId, int gameId, int quantity);
        Task<List<CartItemDto>> GetCartAsync(int userId);
        Task<CheckoutResultDto> CheckoutAsync(int userId);
        Task<List<OrderHistoryDto>> GetOrderHistoryAsync(int userId);
        Task<bool> CancelPreOrderAsync(int orderId);
        Task<SalesReportDto> GetSalesReportAsync(DateTime from, DateTime to);
    }
}
