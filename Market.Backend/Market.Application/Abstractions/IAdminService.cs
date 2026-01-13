using Market.Domain.Models;
using Market.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Abstractions
{
    public interface IAdminService
    {
        Task<List<CommentModerationDto>> GetAllCommentsAsync();
        Task<bool> DeleteCommentAsync(int commentId);
        Task<bool> CreateAnnouncementAsync(CreateNotificationDto dto);
        Task<List<NotificationDto>> GetLatestNotificationsAsync();
        Task<bool> ToggleNotificationStatusAsync(int id);
        Task<List<LogAction>> GetAuditLogsAsync();
        Task<DashBoardStatsDto> GetDashboardStatsAsync();
    }
}
