using Market.Application.Modules.Catalog.ProductCategories.Commands.Create;

namespace Market.Tests.ProductCategoryTests.UnitTests;

public class ProductCategoryUnitTests
{
    private DatabaseContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // svaki test dobije novu bazu
            .Options;

        return new DatabaseContext(options);
    }

    [Fact]
    public async Task Handle_ShouldAddNewCategory()
    {
        // Arrange
        using var context = GetInMemoryDbContext(); // dispose
        var handler = new CreateProductCategoryCommandHandler(context);
        var command = new CreateProductCategoryCommand { Name = "Test Category" };

        // Act
        var resultId = await handler.Handle(command, CancellationToken.None);

        // Assert
        var category = await context.ProductCategories.FindAsync(resultId);
        Assert.NotNull(category);
        Assert.Equal("Test Category", category!.Name);
        // (opcionalno) ako koristiš UTC:
        // Assert.True(category.CreatedAt > DateTime.MinValue);
    }

}