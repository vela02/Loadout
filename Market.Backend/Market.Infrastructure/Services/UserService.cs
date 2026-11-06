using Market.Application;
using Market.Application.Abstractions;
using Market.Domain.Models;
using Market.Infrastructure.Common;
using Market.Application.Common;
using Market.Application.Common.Exceptions;
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

        /// <summary>Admin lista: 5 filtera + sortiranje po koloni + backend paging.</summary>
        public async Task<PageResult<UserDto>> GetUsersPagedAsync(UserFilterDto f)
        {
            var query = _context.Users.Include(u => u.Role).Where(u => !u.IsDeleted).AsQueryable();

            // ---- FILTERI (svaki se primjenjuje samo ako je zadan) ----
            if (!string.IsNullOrWhiteSpace(f.Search))
            {
                var s = f.Search.Trim();
                query = query.Where(u => u.Username.Contains(s) || u.Email.Contains(s));
            }
            if (f.RoleId.HasValue) query = query.Where(u => u.RoleId == f.RoleId);
            if (f.IsEnabled.HasValue) query = query.Where(u => u.IsEnabled == f.IsEnabled);
            if (f.CreatedFrom.HasValue) query = query.Where(u => u.CreatedAt >= f.CreatedFrom.Value.Date);
            if (f.CreatedTo.HasValue) query = query.Where(u => u.CreatedAt < f.CreatedTo.Value.Date.AddDays(1)); // uključivo cijeli dan

            // ---- SORTIRANJE PO KOLONI ----
            bool desc = string.Equals(f.SortDir, "desc", StringComparison.OrdinalIgnoreCase);
            query = (f.SortBy?.ToLower()) switch
            {
                "username"  => desc ? query.OrderByDescending(u => u.Username)  : query.OrderBy(u => u.Username),
                "email"     => desc ? query.OrderByDescending(u => u.Email)     : query.OrderBy(u => u.Email),
                "role"      => desc ? query.OrderByDescending(u => u.Role!.Name) : query.OrderBy(u => u.Role!.Name),
                "lastlogin" => desc ? query.OrderByDescending(u => u.LastLogin) : query.OrderBy(u => u.LastLogin),
                "status"    => desc ? query.OrderByDescending(u => u.IsEnabled) : query.OrderBy(u => u.IsEnabled),
                _           => desc ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
            };

            var projected = query.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                RoleName = u.Role != null ? u.Role.Name : "N/A",
                RoleId = u.RoleId ?? 0,
                IsEnabled = u.IsEnabled,
                CreatedAt = u.CreatedAt,
                LastLogin = u.LastLogin
            });

            // ---- PAGING (Skip/Take u SQL-u, plus ukupan broj) ----
            var paging = new PageRequest { Page = f.Page < 1 ? 1 : f.Page, PageSize = f.PageSize };
            return await PageResult<UserDto>.FromQueryableAsync(projected, paging);
        }

        public async Task<List<RoleDto>> GetRolesAsync()
        {
            return await _context.Roles
                .OrderBy(r => r.Name)
                .Select(r => new RoleDto { Id = r.Id, Name = r.Name })
                .ToListAsync();
        }

        /// <summary>Email i username moraju biti jedinstveni (provjera na backendu).</summary>
        private async Task EnsureUniqueAsync(string username, string email, int? exceptUserId = null)
        {
            var e = email.Trim().ToLower();
            if (await _context.Users.AnyAsync(u => !u.IsDeleted && u.Id != exceptUserId && u.Email.ToLower() == e))
                throw new MarketConflictException("Korisnik s ovim emailom već postoji.");
            if (await _context.Users.AnyAsync(u => !u.IsDeleted && u.Id != exceptUserId && u.Username == username.Trim()))
                throw new MarketConflictException("Korisničko ime je zauzeto.");
        }

        public async Task<bool> CreateUserAsync(UserUpdateDto dto)
        {
            await EnsureUniqueAsync(dto.Username, dto.Email);
            dto.Email = dto.Email.Trim().ToLower(); // login poredi emailove malim slovima
            dto.Username = dto.Username.Trim();

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

            await EnsureUniqueAsync(dto.Username, dto.Email, userId);

            user.Username=dto.Username.Trim();
            user.Email=dto.Email.Trim().ToLower();
            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password); // nova šifra samo ako je unesena
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
