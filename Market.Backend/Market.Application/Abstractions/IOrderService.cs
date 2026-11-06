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
        Task<bool> RemoveFromCartAsync(int userId, int cartItemId);
        Task<bool> SetSavedForLaterAsync(int userId, int cartItemId, bool saved);
        Task<bool> SaveGameForLaterAsync(int userId, int gameId);
        Task<CheckoutResultDto> CheckoutAsync(int userId);
        Task<decimal> CalculateCartTotalAsync(int userId);
        Task<List<OrderHistoryDto>> GetOrderHistoryAsync(int userId);
        Task<bool> CancelPreOrderAsync(int orderId);
        Task<SalesReportDto> GetSalesReportAsync(DateTime from, DateTime to);
    }
}
