using Market.Application.Modules.Wish_List.Queries.GetByUser;
using Market.Application.Modules.Wishlist.Commands.Add;
using Market.Application.Modules.Wishlist.Commands.Remove;


[ApiController]
[Route("api/wishlist")]
public class WishlistController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize]
    //  SECURITY - Kad frontend implementira Login, zamijeni userId parametar sa IAppCurrentUser servisom i na drugim  mjestima gdje je potrebno
    public async Task<IActionResult> Get()
    { 
        return Ok(await mediator.Send(new GetWishlistQuery()));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Add([FromBody] AddWishlistCommand command)
    {
        var success = await mediator.Send(command);

        
        if (!success)
        {
            return BadRequest("Greška: Igra ne postoji ili je već dodana.");
        }        
        return Ok("Uspješno dodano u listu želja.");
    }


    [HttpDelete("{gameId}")]
    [Authorize]
    public async Task<IActionResult> Remove(int gameId) => Ok(await mediator.Send(new RemoveWishlistCommand(gameId)));

   
}