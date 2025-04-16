/*
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shop.API.Controllers;
using Shop.API.DTO;
using Shop.Core.Domain.Entities;
using Shop.Core.Domain.Interfaces;
using Shop.Infrastructure.Services;
using Shop.Core.Application.Commands;
using Xunit;

public class AccountControllerUnitTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<PasswordResetService> _passwordResetServiceMock;
    private readonly Mock<IEmailRepository> _emailRepositoryMock;
    private readonly Mock<ConfirmAccountCommand> _confirmAccountCommandMock;
    private readonly Mock<ResetPasswordCommand> _resetPasswordCommandMock;
    private readonly AccountController _controller;

    public AccountControllerUnitTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordResetServiceMock = new Mock<PasswordResetService>();
        _emailRepositoryMock = new Mock<IEmailRepository>();
        _confirmAccountCommandMock = new Mock<ConfirmAccountCommand>();
        _resetPasswordCommandMock = new Mock<ResetPasswordCommand>();

        _controller = new AccountController(
            _userRepositoryMock.Object,
            _passwordResetServiceMock.Object,
            _emailRepositoryMock.Object,
            _confirmAccountCommandMock.Object,
            _resetPasswordCommandMock.Object);
    }

    [Fact]
    public async Task RequestPasswordReset_ShouldReturnOk_WhenEmailExists()
    {
        var user = new User { Email = "test@example.com" };
        _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        var request = new PasswordResetRequest { Email = "test@example.com" };

        var result = await _controller.RequestPasswordReset(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("Сброс пароля отправлен на почту", okResult.Value.ToString());
    }

    [Fact]
    public async Task RequestPasswordReset_ShouldReturnBadRequest_WhenEmailNotExists()
    {
        var request = new PasswordResetRequest { Email = "notfound@example.com" };
        _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(request.Email)).ReturnsAsync((User)null);

        var result = await _controller.RequestPasswordReset(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ConfirmAccount_ShouldReturnOk_WhenTokenIsValid()
    {
        var token = "valid-token";
        _confirmAccountCommandMock.Setup(cmd => cmd.ExecuteAsync(token)).Returns(Task.CompletedTask);

        var result = await _controller.ConfirmAccount(token);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("Аккаунт подтвержден", okResult.Value.ToString());
    }

    [Fact]
    public async Task ResetPassword_ShouldReturnOk_WhenTokenAndPasswordAreValid()
    {
        var request = new ResetPasswordRequest { Token = "valid-token", Password = "new-password" };
        _resetPasswordCommandMock.Setup(cmd => cmd.ExecuteAsync(request.Token, request.Password)).Returns(Task.CompletedTask);

        var result = await _controller.ResetPassword(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("Пароль успешно изменен", okResult.Value.ToString());
    }
}
*/