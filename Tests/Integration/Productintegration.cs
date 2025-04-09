using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;
using FluentAssertions;
using Shop.API;

public class ProductsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductsControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllVisibleProducts_ShouldReturnOk()
    {
        var url = "/api/products";

        var response = await _client.GetAsync(url);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        var productId = Guid.NewGuid();
        var url = $"/api/products/{productId}";

        var response = await _client.GetAsync(url);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddProduct_ShouldReturnCreatedAt_WhenValidRequest()
    {
        var url = "/api/products";
        var request = new
        {
            Name = "New Product",
            Description = "Product Description",
            Price = 120.00
        };
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(url, content);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnNoContent_WhenProductDeleted()
    {
        var productId = "existing-product-id"; 
        var url = $"/api/products/{productId}";

        var response = await _client.DeleteAsync(url);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
