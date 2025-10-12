using Market.Features.Common;
using Market.Features.ProductCategories.Commands.Create;
using Market.Features.ProductCategories.Commands.Delete;
using Market.Features.ProductCategories.Commands.Update;
using Market.Features.ProductCategories.Queries.GetById;
using Market.Features.ProductCategories.Queries.List;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductCategoryController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> CreateProductCategory(CreateProductCategoryCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id}")]
    public async Task Update(int id, UpdateProductCategoryCommand command, CancellationToken ct)
    {
        // ID iz rute ima prioritet
        command.Id = id;
        await sender.Send(command, ct);
        // bez return -> 204 No Content
    }

    [HttpDelete("{id}")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteProductCategoryCommand { Id = id }, ct);
        // bez return -> 204 No Content
    }

    [HttpGet("{id}")]
    public async Task<GetProductCategoryByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var category = await sender.Send(new GetProductCategoryByIdQuery { Id = id }, ct);
        return category; // NotFoundException -> 404 preko middleware-a
    }

    [HttpGet]
    public async Task<PageResult<ListProductCategoriesQueryDto>> List([FromQuery] ListProductCategoriesQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return result;
    }
}
