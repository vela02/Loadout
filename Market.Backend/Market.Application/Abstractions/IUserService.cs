using Market.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Abstractions
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync(string? searchTerm);
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<bool> CreateUserAsync(UserUpdateDto dto);
        Task<bool> UpdateUserAsync(int id, UserUpdateDto dto);
        Task<bool> DeleteUserAsync(int id);
        Task<List<OrderHistoryDto>> GetUserPurchaseHistoryAsync(int userId);
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto);
    }
}
