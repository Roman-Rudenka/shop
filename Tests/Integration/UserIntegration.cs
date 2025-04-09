using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;
using FluentAssertions;
using Shop.API;

public class UsersControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public UsersControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenValidRequest()
    {
        var url = "/api/users/register";
        var request = new
        {
            Name = "John",
            Email = "john@example.com",
            Password = "password123",
            Role = "Admin"
        };
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(url, content);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenValidCredentials()
    {
        var url = "/api/users/login";
        var request = new
        {
            Email = "john@example.com",
            Password = "password123"
        };
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(url, content);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ActivateUser_ShouldReturnOk_WhenUserExists()
    {
        var userId = "existing-user-id"; 
        var url = $"/api/users/{userId}/activate";

        var response = await _client.PostAsync(url, null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeactivateUser_ShouldReturnOk_WhenUserExists()
    {
        var userId = "existing-user-id";
        var url = $"/api/users/{userId}/deactivate";

        var response = await _client.PostAsync(url, null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
