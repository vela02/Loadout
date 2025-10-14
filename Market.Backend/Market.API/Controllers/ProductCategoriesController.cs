using Market.Application.Features.ProductCategories.Commands.Delete;
using Market.Application.Features.ProductCategories.Commands.Status.Disable;
using Market.Application.Features.ProductCategories.Commands.Status.Enable;
using Market.Application.Features.ProductCategories.Commands.Create;
using Market.Application.Features.ProductCategories.Commands.Update;
using Market.Application.Features.ProductCategories.Queries.GetById;
using Market.Application.Features.ProductCategories.Queries.List;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductCategoriesController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> CreateProductCategory(CreateProductCategoryCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task Update(int id, UpdateProductCategoryCommand command, CancellationToken ct)
    {
        // ID iz rute ima prioritet
        command.Id = id;
        await sender.Send(command, ct);
        // bez return -> 204 No Content
    }

    [HttpDelete("{id:int}")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteProductCategoryCommand { Id = id }, ct);
        // bez return -> 204 No Content
    }

    [HttpGet("{id:int}")]
    public async Task<GetProductCategoryByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var category = await sender.Send(new GetProductCategoryByIdQuery { Id = id }, ct);
        return category; // ako je NotFoundException -> 404 preko middleware-a
    }

    [HttpGet]
    public async Task<PageResult<ListProductCategoriesQueryDto>> List([FromQuery] ListProductCategoriesQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return result;
    }

    [HttpPut("{id:int}/disable")]
    public async Task Disable(int id, CancellationToken ct)
    {
        await sender.Send(new DisableProductCategoryCommand { Id = id }, ct);
        // bez return -> 204 No Content
    }

    [HttpPut("{id:int}/enable")]
    public async Task Enable(int id, CancellationToken ct)
    {
        await sender.Send(new EnableProductCategoryCommand { Id = id }, ct);
        // bez return -> 204 No Content
    }

}
