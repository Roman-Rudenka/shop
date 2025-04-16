/*
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shop.API.Controllers;
using Shop.API.DTO;
using Shop.Core.Domain.Interfaces;
using Shop.Core.Application.Commands;
using Shop.Core.Application.Queries;
using Xunit;
using Shop.Core.Domain.Entities;
using MediatR;

public class UsersControllerUnitTests
{
    private readonly Mock<IMediator> _userCommandsMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly UsersController _controller;

    public UsersControllerUnitTests()
    {
        _userCommandsMock = new Mock<IMediator>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _controller = new UsersController(_userCommandsMock.Object, _userQueriesMock.Object, _productRepositoryMock.Object);
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenValidRequest()
    {
        var request = new RegisterUserRequest
        {
            Name = "John",
            Email = "john@example.com",
            Password = "password123",
            Role = "Admin"
        };
        _userCommandsMock.Setup(c => c.RegisterUserAsync(request.Name, request.Email, request.Password, UserRole.Admin))
            .Returns(Task.CompletedTask);


        var result = await _controller.Register(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("успешно зарегистрирован", okResult.Value.ToString());
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenFieldsAreMissing()
    {
        var request = new RegisterUserRequest { Name = "", Email = "", Password = "", Role = "" };

        var result = await _controller.Register(request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("Все поля обязательны", badRequestResult.Value.ToString());
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenValidCredentials()
    {
        var request = new LoginRequest { Email = "john@example.com", Password = "password123" };
        var token = "valid-token";
        _userCommandsMock.Setup(c => c.LoginAsync(request.Email, request.Password)).ReturnsAsync(token);

        var result = await _controller.Login(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(token, ((dynamic)okResult.Value).Token);
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnOk_WithUserList()
    {
        var users = new List<User> { new User { Id = Guid.NewGuid(), Name = "John" } };
        _userQueriesMock.Setup(q => q.GetAllUsersAsync()).ReturnsAsync(users);

        var result = await _controller.GetAllUsers();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(users, okResult.Value);
    }

    [Fact]
    public async Task ActivateUser_ShouldReturnOk_WhenUserExists()
    {
        var userId = Guid.NewGuid();

        _userCommandsMock.Setup(c => c.ActivateUserAsync(userId)).Returns(Task.CompletedTask);
        _productRepositoryMock.Setup(r => r.ShowProductsByPublisherAsync(userId)).Returns(Task.CompletedTask);

        var result = await _controller.ActivateUser(userId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("активирован", okResult.Value.ToString());
    }

    [Fact]
    public async Task DeactivateUser_ShouldReturnOk_WhenUserExists()
    {
        var userId = Guid.NewGuid();

        _userCommandsMock.Setup(c => c.DeactivateUserAsync(userId)).Returns(Task.CompletedTask);
        _productRepositoryMock.Setup(r => r.HideProductsByPublisherAsync(userId)).Returns(Task.CompletedTask);

        var result = await _controller.DeactivateUser(userId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("деактивирован", okResult.Value.ToString());
    }
}
*/