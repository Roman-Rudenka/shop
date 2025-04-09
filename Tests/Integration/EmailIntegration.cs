using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;
using FluentAssertions;
using Shop.API;

public class AccountControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AccountControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RequestPasswordReset_ShouldReturnOk_WhenEmailIsValid()
    {
        var url = "/api/account/request-password-reset";
        var request = new { Email = "test@example.com" };
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(url, content);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ConfirmAccount_ShouldReturnOk_WhenTokenIsValid()
    {
        var token = "valid-token";
        var url = $"/api/account/confirm-account?token={token}";

        var response = await _client.PostAsync(url, null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ResetPassword_ShouldReturnOk_WhenValidRequest()
    {
        var url = "/api/account/reset-password";
        var request = new
        {
            Token = "valid-token",
            Password = "new-password"
        };
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(url, content);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SendConfirmationEmail_ShouldReturnOk_WhenEmailExists()
    {
        var url = "/api/account/send-confirmation-email";
        var request = new { Email = "test@example.com" };
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(url, content);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
