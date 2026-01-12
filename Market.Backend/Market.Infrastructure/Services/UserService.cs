using Market.Application;
using Market.Application.Abstractions;
using Market.Domain.Models;
using Market.Infrastructure.Common;
using Market.Shared.Dtos;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Infrastructure.Services
{
    public class UserService:IUserService
    {
        private readonly LoadoutDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;

        public UserService(LoadoutDbContext context, IPasswordHasher<User> passwordHasher, IJwtTokenService jwtTokenService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<List<UserDto>> GetAllUsersAsync(string? searchTerm)
        {
            var query=_context.Users.Include(u=>u.Role).Where(u =>!u.IsDeleted).AsQueryable();
            if(!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => u.Username.Contains(searchTerm) || u.Email.Contains(searchTerm));
            }
            return await query.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                RoleName = u.Role!=null?u.Role.Name:"N/A",
                RoleId =u.RoleId??0,
                IsEnabled = u.IsEnabled
            }).ToListAsync();
        }

        public async Task<bool> CreateUserAsync(UserUpdateDto dto)
        {
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                RoleId = dto.RoleId,
                IsEnabled = dto.IsEnabled,
                CreatedAt = DateTime.UtcNow
            };
            if (!string.IsNullOrEmpty(dto.Password))
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
            }
            else
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, "Default123!");
            }

            _context.Users.Add(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateUserAsync(int userId, UserUpdateDto dto)
        {
            var user=await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.Username=dto.Username;
            user.Email=dto.Email;
            user.RoleId=dto.RoleId;
            user.IsEnabled=dto.IsEnabled;


            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user= await _context.Users.FindAsync(userId);
            if (user == null) return false;
            user.IsDeleted = true; //ovo vodimo kao soft delete
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Where(u=> u.Id==userId)
                .Select(u=> new UserDto
                {
                    Id=u.Id,
                    Username=u.Username,
                    Email=u.Email,
                    RoleId=u.RoleId??0,
                    RoleName=u.Role.Name,
                    IsEnabled=u.IsEnabled
                }).FirstOrDefaultAsync();
        }

        public async Task<List<OrderHistoryDto>> GetUserPurchaseHistoryAsync(int userId)
        {
            return await _context.Orders
                .Where(o=> o.UserId==userId)
                .Include(o=> o.Status)
                .Include(o=> o.OrderGames).ThenInclude(og=> og.Game)
                .Select(o => new OrderHistoryDto
                {
                    Id=o.Id,
                    Date=o.Date,
                    TotalAmount=o.TotalAmount??0,
                    Status=o.Status.Name,
                    Games=o.OrderGames.Select(og=> new BoughtGameDto
                    {
                        Title=og.Game.Title,
                        PriceAtPurchase=og.PriceAtPurchase??0,

                    }).ToList()
                }).ToListAsync();
        }
        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto)
        {
            // 1. Pronađi korisnika po emailu
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == dto.Email && !u.IsDeleted);

            if (user == null) return null;

            // 2. Provjeri hash lozinke
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            // 3. Generiši JWT Token koristeći tvoj servis
            // Pretpostavljam da tvoj IJwtTokenService ima metodu CreateToken ili slično
            var tokenPair = _jwtTokenService.IssueTokens(user);

            return new LoginResponseDto
            {
                AccessToken = tokenPair.AccessToken,
                RefreshToken = tokenPair.RefreshTokenRaw,
                Username = user.Username,
                Role = user.Role?.Name ?? "User"
            };
        }

    }
}
