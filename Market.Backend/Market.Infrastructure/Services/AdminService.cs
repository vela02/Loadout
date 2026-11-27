using Market.Application;
using Market.Application.Abstractions;
using Market.Domain.Models;
using Market.Shared.Dtos;
using Market.Application.Common;
using Microsoft.EntityFrameworkCore;
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
        /// <summary>
        /// Moderacija recenzija: filteri (igra, korisnik, min. ocjena, period, samo prijavljene)
        /// + backend paging. Prijavljene recenzije su uvijek na vrhu.
        /// </summary>
        public async Task<PageResult<CommentModerationDto>> GetCommentsPagedAsync(ReviewFilterDto f)
        {
            var query = _context.Comments.AsQueryable();

            if (!string.IsNullOrWhiteSpace(f.Game))
                query = query.Where(c => c.Game != null && c.Game.Title.Contains(f.Game.Trim()));
            if (!string.IsNullOrWhiteSpace(f.User))
                query = query.Where(c => c.User != null && c.User.Username.Contains(f.User.Trim()));
            if (f.MinRating.HasValue)
                // ocjena se čuva u tabeli Rating (isti korisnik + ista igra)
                query = query.Where(c => _context.Ratings.Any(r =>
                    r.UserId == c.UserId && r.GameId == c.GameId && r.Stars >= f.MinRating.Value));
            if (f.CreatedFrom.HasValue)
                query = query.Where(c => c.CreatedAt >= f.CreatedFrom.Value.Date);
            if (f.CreatedTo.HasValue)
                query = query.Where(c => c.CreatedAt < f.CreatedTo.Value.Date.AddDays(1));
            if (f.ReportedOnly == true)
                query = query.Where(c => c.IsReported);

            var projected = query
                .OrderByDescending(c => c.IsReported)      // prijavljene prve -> admin ih odmah vidi
                .ThenByDescending(c => c.CreatedAt)
                .Select(c => new CommentModerationDto
                {
                    Id = c.Id,
                    Username = c.User != null ? c.User.Username : "Nepoznat korisnik",
                    GameTitle = c.Game != null ? c.Game.Title : "Nepoznata igra",
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    GameId = c.GameId,
                    UserId = c.UserId,
                    IsReported = c.IsReported,
                    Rating = _context.Ratings
                        .Where(r => r.UserId == c.UserId && r.GameId == c.GameId)
                        .OrderByDescending(r => r.CreatedAt)
                        .Select(r => (int?)r.Stars)
                        .FirstOrDefault()
                });

            var paging = new PageRequest { Page = f.Page < 1 ? 1 : f.Page, PageSize = f.PageSize };
            return await PageResult<CommentModerationDto>.FromQueryableAsync(projected, paging);
        }

        /// <summary>Admin je pregledao prijavljenu recenziju i ocijenio da je u redu -> ukloni oznaku prijave.</summary>
        public async Task<bool> DismissCommentReportAsync(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null) return false;

            comment.IsReported = false;
            await LogAdminAction("UPDATE", "Comments", commentId, "Administrator dismissed the report for a comment.");
            await _context.SaveChangesAsync();
            return true;
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
        /// <summary>Admin lista globalnih vijesti (UserId = null): 5 filtera + backend paging, najnovije prve.</summary>
        public async Task<PageResult<AnnouncementAdminDto>> GetAnnouncementsPagedAsync(AnnouncementFilterDto f)
        {
            var query = _context.Notifications.Where(n => n.UserId == null).AsQueryable();

            if (!string.IsNullOrWhiteSpace(f.Title))
                query = query.Where(n => n.Title.Contains(f.Title.Trim()));
            if (!string.IsNullOrWhiteSpace(f.Type))
                query = query.Where(n => n.Type == f.Type);
            if (f.IsActive.HasValue)
                query = query.Where(n => n.IsActive == f.IsActive.Value);
            if (f.CreatedFrom.HasValue)
                query = query.Where(n => n.CreatedAt >= f.CreatedFrom.Value.Date);
            if (f.CreatedTo.HasValue)
                query = query.Where(n => n.CreatedAt < f.CreatedTo.Value.Date.AddDays(1)); // uključivo cijeli dan

            var projected = query
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new AnnouncementAdminDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    Type = n.Type,
                    IsActive = n.IsActive,
                    CreatedAt = n.CreatedAt,
                    PublishAt = n.PublishAt,
                    ExpiresAt = n.ExpiresAt
                });

            var paging = new PageRequest { Page = f.Page < 1 ? 1 : f.Page, PageSize = f.PageSize };
            return await PageResult<AnnouncementAdminDto>.FromQueryableAsync(projected, paging);
        }

        public async Task<bool> UpdateAnnouncementAsync(int id, CreateNotificationDto dto)
        {
            var n = await _context.Notifications.FirstOrDefaultAsync(x => x.Id == id && x.UserId == null);
            if (n == null) return false;

            n.Title = dto.Title.Trim();
            n.Message = Market.Infrastructure.Common.HtmlSanitizer.Sanitize(dto.Message); // ukloni opasan HTML
            n.Type = dto.Type;
            n.IsActive = dto.IsActive;
            n.PublishAt = dto.PublishAt;
            n.ExpiresAt = dto.ExpiresAt;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAnnouncementAsync(int id)
        {
            var n = await _context.Notifications.FirstOrDefaultAsync(x => x.Id == id && x.UserId == null);
            if (n == null) return false;

            _context.Notifications.Remove(n);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CreateAnnouncementAsync(CreateNotificationDto dto)
        {
            var notification = new Notification
            {
                Title = dto.Title,
                Message = Market.Infrastructure.Common.HtmlSanitizer.Sanitize(dto.Message), // ukloni opasan HTML
                Type = dto.Type,
                CreatedAt = DateTime.Now,
                IsRead = false,
                IsActive = dto.IsActive,
                PublishAt = dto.PublishAt,
                ExpiresAt = dto.ExpiresAt,
                UserId = null
            };

            _context.Notifications.Add(notification);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<NotificationDto>> GetLatestNotificationsAsync()
        {
            return await _context.Notifications
                // samo aktivne vijesti za sve, čiji je termin objave došao, a nisu istekle
                .Where(n => n.UserId == null && n.IsActive
                            && (n.PublishAt == null || n.PublishAt <= DateTime.Now)
                            && (n.ExpiresAt == null || n.ExpiresAt > DateTime.Now))
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
