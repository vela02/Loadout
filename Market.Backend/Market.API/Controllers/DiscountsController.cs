using Market.Application.Modules.Catalog.Discounts.Commands.Create;
using Market.Application.Modules.Catalog.Discounts.Commands.Delete;

namespace Market.API.Controllers
{
    [Route("api/discounts")]
    [ApiController]
    public class DiscountsController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
         [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> Create([FromBody] CreateDiscountCommand command, CancellationToken ct)
        {
            var id = await mediator.Send(command, ct);
            return CreatedAtAction(nameof(Create), new { id = id }, id);
        }

        [HttpDelete("{id}")]
         [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id, CancellationToken ct)
        {
            await mediator.Send(new DeleteDiscountCommand { Id = id }, ct);
            return NoContent(); 
        }
    }
}
