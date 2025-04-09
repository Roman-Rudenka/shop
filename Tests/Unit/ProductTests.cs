using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shop.API.Controllers;
using Shop.Core.Entities;
using Shop.Core.UseCases.Commands;
using Shop.Core.UseCases.Queries;
using Xunit;

public class ProductsControllerUnitTests
{
    private readonly Mock<ProductCommands> _productCommandsMock;
    private readonly Mock<ProductQueries> _productQueriesMock;
    private readonly ProductsController _controller;

    public ProductsControllerUnitTests()
    {
        _productCommandsMock = new Mock<ProductCommands>();
        _productQueriesMock = new Mock<ProductQueries>();
        _controller = new ProductsController(_productCommandsMock.Object, _productQueriesMock.Object);
    }

    [Fact]
    public async Task GetAllVisibleProducts_ShouldReturnOk_WithProductList()
    {
        var products = new List<Product> { new Product { Name = "Product1", Price = 100 } };
        _productQueriesMock.Setup(q => q.GetAllVisibleProductsAsync()).ReturnsAsync(products);

        var result = await _controller.GetAllVisibleProducts();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(products, okResult.Value);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        var productId = Guid.NewGuid();
        _productQueriesMock.Setup(q => q.GetProductByIdAsync(productId)).ReturnsAsync((Product)null);

        var result = await _controller.GetProductById(productId);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnOk_WhenProductExists()
    {
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, Name = "Product1", Price = 100 };
        _productQueriesMock.Setup(q => q.GetProductByIdAsync(productId)).ReturnsAsync(product);

        var result = await _controller.GetProductById(productId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(product, okResult.Value);
    }

    [Fact]
    public async Task AddProduct_ShouldReturnCreatedAtAction_WhenProductIsValid()
    {
        var product = new Product { Name = "Product1", Description = "Desc", Price = 100 };
        var userId = Guid.NewGuid();
        _productCommandsMock.Setup(c => c.AddProductAsync(product.Name, product.Description, product.Price, userId))
            .Returns(Task.CompletedTask);


        var result = await _controller.AddProduct(product);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(product, createdResult.Value);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnNoContent_WhenProductDeleted()
    {
        var productId = Guid.NewGuid();
        _productCommandsMock.Setup(c => c.DeleteProductAsync(productId)).Returns(Task.CompletedTask);

        var result = await _controller.DeleteProduct(productId);

        Assert.IsType<NoContentResult>(result);
    }
}
