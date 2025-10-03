using Market.Features.ProductCategories.CreateProductCategory;
using Market.Features.ProductCategories.GetProductCategoryById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
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
    }
}