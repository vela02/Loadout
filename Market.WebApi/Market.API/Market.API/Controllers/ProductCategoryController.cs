using Market.Features.Common;
using Market.Features.ProductCategories.CreateProductCategory;
using Market.Features.ProductCategories.GetProductCategoryById;
using Market.Features.ProductCategories.ListProductCategories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Market.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductCategoryController(ISender sender) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<int>> CreateProductCategory(CreateProductCategoryCommand command)
        {
            try
            {
                int productCategoryId = await sender.Send(command);
                return Ok(productCategoryId);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var category = await sender.Send(new GetProductCategoryByIdQuery { Id = id }, cancellationToken);

            if (category == null)
                return NotFound($"Product category with Id {id} not found.");

            return Ok(category);
        }

        [HttpGet]
        public async Task<ActionResult<PageResult<ListProductCategoriesItem>>> List(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] bool? onlyEnabled = null)
        {
            var result = await sender.Send(new ListProductCategoriesQuery
            {
                Paging = new Shared.Dtos.PageRequest { Page = page, PageSize = pageSize },
                Search = search,
                OnlyEnabled = onlyEnabled
            });
            return Ok(result);
        }

    }
}