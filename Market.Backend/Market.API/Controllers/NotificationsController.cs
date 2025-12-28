using Market.Application.Modules.Notifications.Commands.MarkAsRead;
using Market.Application.Modules.Notifications.Queries.GetByUser;
using MediatR;


    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController(IMediator mediator) : ControllerBase
    {

     [HttpGet("{userId}")]
     [AllowAnonymous]
     public async Task<IActionResult> GetNotifications(int userId)
     {
        return Ok(await mediator.Send(new GetUserNotificationsQuery(userId)));
     }

     [HttpPost("/read/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> MarkAsRead(int id)
        {

            var result = await mediator.Send(new MarkNotificationAsReadCommand(id));

            if (!result) return NotFound("Notifikacija nije pronađena.");

            return Ok("Notifikacija označena kao pročitana.");
        }
    }

