using Market.Application.Modules.Notifications.Commands.MarkAsRead;
using Market.Application.Modules.Notifications.Queries.GetByUser;
using Market.Application.Modules.Wish_List.Queries.GetByUser; 
using Market.Application.Modules.Wishlist.Commands.Add;
using Market.Application.Modules.Wishlist.Commands.Remove;


[ApiController]
[Route("api/wishlist")]
public class WishlistController(IMediator mediator) : ControllerBase
{
    [HttpGet("{userId}")]
    [AllowAnonymous]
    public async Task<IActionResult> Get(int userId) => Ok(await mediator.Send(new GetWishlistQuery(userId)));

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Add([FromBody] AddWishlistCommand command) => Ok(await mediator.Send(command));

    [HttpDelete("{userId}/{gameId}")]
    [AllowAnonymous]
    public async Task<IActionResult> Remove(int userId, int gameId) => Ok(await mediator.Send(new RemoveWishlistCommand(userId, gameId)));

    // might add next two in NotificationsController
    [HttpGet("notifications/{userId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetNotifications(int userId) => Ok(await mediator.Send(new GetUserNotificationsQuery(userId)));

    [HttpPost("notifications/read/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        
        var result = await mediator.Send(new MarkNotificationAsReadCommand(id));

        if (!result) return NotFound("Notifikacija nije pronađena.");

        return Ok("Notifikacija označena kao pročitana.");
    }
}