using Market.Application.Modules.Sales.Orders.Queries.List;
using Market.Application.Modules.Sales.Orders.Commands.Status;


namespace Market.API.Controllers
{
    
    [Route("api/admin/orders")]
    [ApiController]
    public class AdminOrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminOrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // list of all orders
        [HttpGet("list")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<PageResult<ListOrdersQueryDto>>> GetAll([FromQuery] ListOrdersQuery query)
        {
            return Ok(await _mediator.Send(query));
        }

        // status update
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> UpdateStatus(int id, [FromBody] ChangeOrderStatusCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }
    }
}