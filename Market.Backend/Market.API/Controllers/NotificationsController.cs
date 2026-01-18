using Market.Application.Abstractions;
using Market.Application.Modules.Notifications.Commands.MarkAsRead;
using Market.Application.Modules.Notifications.Queries.GetByUser;
// Makni "using Market.Shared.Dtos;" ako pravi problem, ili koristi punu putanju ispod
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Market.API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController(IMediator mediator, IAdminService adminService) : ControllerBase
    {
        // OVDJE SMO STAVILI PUNU PUTANJU: Market.Shared.Dtos.NotificationDto
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Market.Shared.Dtos.NotificationDto>>> GetGlobalAnnouncements()
        {
            var news = await adminService.GetLatestNotificationsAsync();
            return Ok(news);
        }

        [HttpGet("my-notifications")]
        [Authorize]
        public async Task<IActionResult> GetNotifications()
        {
           
            return Ok(await mediator.Send(new GetUserNotificationsQuery()));
        }

        [HttpPost("read/{id}")]
        [Authorize]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var result = await mediator.Send(new MarkNotificationAsReadCommand(id));
            if (!result) return NotFound("Notifikacija nije pronađena.");
            return Ok("Notifikacija označena kao pročitana.");
        }
    }
}