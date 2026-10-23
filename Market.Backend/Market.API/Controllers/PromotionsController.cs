using Market.Application.Modules.Catalog.Promotions.Commands.Create;
using Market.Application.Modules.Catalog.Promotions.Commands.Delete;
using Market.Application.Modules.Catalog.Promotions.Queries.GetOptions;
using Market.Application.Modules.Catalog.Promotions.Queries.List;

namespace Market.API.Controllers
{
    [Route("api/admin/promotions")]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    public class PromotionsController(IMediator mediator) : ControllerBase
    {
        // GET: api/admin/promotions
        [HttpGet]
        public async Task<List<ListPromotionsQueryDto>> List(CancellationToken ct)
        {
            return await mediator.Send(new ListPromotionsQuery(), ct);
        }

        // GET: api/admin/promotions/options
        [HttpGet("options")]
        public async Task<GetPromotionOptionsQueryDto> Options(CancellationToken ct)
        {
            return await mediator.Send(new GetPromotionOptionsQuery(), ct);
        }

        // POST: api/admin/promotions
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreatePromotionCommand command, CancellationToken ct)
        {
            var id = await mediator.Send(command, ct);
            return Ok(id);
        }

        // DELETE: api/admin/promotions/{id}
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id, CancellationToken ct)
        {
            await mediator.Send(new DeletePromotionCommand { Id = id }, ct);
            return NoContent();
        }
    }
}
