using Market.Application;
using Market.Application.Modules.Catalog.Products.Commands.Create;
using Market.Application.Modules.Catalog.Products.Commands.Delete;
using Market.Application.Modules.Catalog.Products.Commands.Update;
using Market.Application.Modules.Catalog.Products.Queries.List;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Market.API.Controllers
{
    [Route("api/admin/products")]
    [ApiController]
    public class AdminProductsController : ControllerBase
    {
        private readonly LoadoutDbContext _context;
        private readonly IMediator _mediator;

        public AdminProductsController(LoadoutDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        // 1. GET: api/admin/products
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        // filters: search, categoryId, isEnabled, minPrice, maxPrice + paging.page / paging.pageSize
        public async Task<ActionResult<PageResult<ListProductsQueryDto>>> GetAll([FromQuery] ListProductsQuery query)
        {
            return Ok(await _mediator.Send(query));
        }

        // 2. POST: api/admin/products 
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Create([FromBody] CreateProductCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(new { id });
        }

        // 3. PUT 
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateProductCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }

        // 4. DELETE
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteProductCommand { Id = id });
            return NoContent();
        }


        // GET:
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> GetById(int id)
        {
           
            var game = await _context.Games
                .Include(g => g.Category)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
            {
                return NotFound();
            }
            
            return Ok(new
            {
                id = game.Id,
                name = game.Title,           
                description = game.Description,
                price = game.Price ?? 0,     
                stockQuantity = 999,         
                categoryName = game.Category != null ? game.Category.Name : "-",
                categoryId = game.CategoryId,
                isEnabled = game.IsEnabled

            });
        }
    }
}

