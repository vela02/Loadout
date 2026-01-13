using Market.Application;
using Market.Application.Abstractions;
using Market.Domain.Models;
using Market.Shared.Dtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Infrastructure.Services
{
    public class AdminService:IAdminService
    {
        private readonly LoadoutDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminService(LoadoutDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<CommentModerationDto>> GetAllCommentsAsync()
        {
            return await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Game)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentModerationDto
                {
                    Id = c.Id,
                    Username = c.User != null ? c.User.Username : "Nepoznat korisnik",
                    GameTitle = c.Game != null ? c.Game.Title : "Nepoznata igra",
                    Content = c.Content, // Ovdje pazi, ako ti se polje zove Text ili Message, promijeni
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();
        }
        public async Task<bool> DeleteCommentAsync(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null) return false;

            // Korištenje proširenog loga:
            await LogAdminAction("DELETE", "Comments", commentId, $"Administrator deleted comment: '{comment.Content}'");

            _context.Comments.Remove(comment);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> CreateAnnouncementAsync(CreateNotificationDto dto)
        {
            var notification = new Notification
            {
                Title = dto.Title,
                Message = dto.Message,
                Type = dto.Type,
                CreatedAt = DateTime.Now,
                IsRead = false,
                IsActive = true,
                UserId = null
            };

            _context.Notifications.Add(notification);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<NotificationDto>> GetLatestNotificationsAsync()
        {
            return await _context.Notifications
                .Where(n => n.UserId == null && n.IsActive) //samo aktivne vijesti za sve
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    Type = n.Type,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<bool> ToggleNotificationStatusAsync(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return false;

            // Ako je true postat će false, ako je false postat će true
            notification.IsActive = !notification.IsActive;

            return await _context.SaveChangesAsync() > 0;
        }
        private async Task LogAdminAction(string type, string entity, int? entityId, string message)
        {
            var log = new LogAction
            {
                AdminUsername = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "AdminRijad",
                ActionType = type,
                EntityName = entity,
                EntityId = entityId,
                Message = message,
                Timestamp = DateTime.Now,
                IpAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString()
            };

            _context.LogActions.Add(log);
            await _context.SaveChangesAsync();
        }
        public async Task<List<LogAction>> GetAuditLogsAsync()
        {
            return await _context.LogActions
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();
        }

        public async Task<DashBoardStatsDto> GetDashboardStatsAsync()
        {
            await LogAdminAction("VIEW", "Statistics", null, "Administrator viewed dashboard statistics.");
            return new DashBoardStatsDto
            {
                // Broj svih korisnika koji nisu obrisani
                TotalUsers = await _context.Users.CountAsync(u => !u.IsDeleted),

                // Suma zarade od svih završenih narudžbi (StatusId 1 = Placeno)
                TotalEarnings = await _context.Orders
                    .Where(o => o.StatusId == 1)
                    .SumAsync(o => o.TotalAmount ?? 0),

                // Ukupan broj prodatih digitalnih ključeva
                TotalLicensesSold = await _context.Licenses.CountAsync(),

                // Broj aktivnih rezervacija (StatusId 4 = Pre-ordered)
                ActivePreOrders = await _context.Orders.CountAsync(o => o.StatusId == 4)
            };
        }
    }
}
