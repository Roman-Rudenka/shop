/*
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shop.API.Controllers;
using Shop.Core.Domain.Entities;
using Shop.Core.Application.Commands;
using Shop.Core.Application.Queries;
using Xunit;
using MediatR;

public class ProductsControllerUnitTests
{
    private readonly Mock<IMediator> _productCommandsMock;
    private readonly ProductsController _controller;

    public ProductsControllerUnitTests()
    {
        _productCommandsMock = new Mock<IMediator>();
        _controller = new ProductsController(_productCommandsMock.Object, _productQueriesMock.Object);
    }

    [Fact]
    public async Task GetAllVisibleProducts_ShouldReturnOk_WithProductList()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Name = "Product1", Price = 100 },
            new Product { Name = "Product2", Price = 200 }
        };
        _productQueriesMock.Setup(q => q.GetAllVisibleProductsAsync()).ReturnsAsync(products);

        // Act
        var result = await _controller.GetAllVisibleProducts();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(products, okResult.Value);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _productQueriesMock.Setup(q => q.GetProductByIdAsync(productId)).ReturnsAsync((Product)null);

        // Act
        var result = await _controller.GetProductById(productId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnOk_WhenProductExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, Name = "Product1", Price = 100 };
        _productQueriesMock.Setup(q => q.GetProductByIdAsync(productId)).ReturnsAsync(product);

        // Act
        var result = await _controller.GetProductById(productId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(product, okResult.Value);
    }

    [Fact]
    public async Task AddProduct_ShouldReturnCreatedAtAction_WhenProductIsValid()
    {
        // Arrange
        var product = new Product { Name = "Product1", Description = "Desc", Price = 100 };
        var userId = Guid.NewGuid(); // �������� �� ��������� userId
        _productCommandsMock.Setup(c => c.AddProductAsync(product.Name, product.Description, product.Price, userId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AddProduct(product);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.NotNull(createdResult.Value);
        Assert.Equal(product, createdResult.Value);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnNoContent_WhenProductDeleted()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _productCommandsMock.Setup(c => c.DeleteProductAsync(productId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteProduct(productId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}
*/