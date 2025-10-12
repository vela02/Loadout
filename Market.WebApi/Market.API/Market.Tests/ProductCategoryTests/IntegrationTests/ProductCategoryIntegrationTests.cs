using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Json;

namespace Market.Tests.ProductCategoryTests.IntegrationTests;

public class ProductCategoryIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductCategoryIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_CreateProductCategory_ShouldReturnCreated()
    {
        // Arrange
        var request = new
        {
            Name = "Integration Test Category"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/ProductCategory", request);

        // Assert
        response.EnsureSuccessStatusCode();

        var categoryId = await response.Content.ReadFromJsonAsync<int?>();
        Assert.NotNull(categoryId);
        Assert.NotEqual(0, categoryId);
    }
}