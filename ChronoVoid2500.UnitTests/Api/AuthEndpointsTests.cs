using System.Net;
using System.Net.Http.Json;
using ChronoVoid2500.UnitTests.TestInfrastructure;
using FluentAssertions;

namespace ChronoVoid2500.UnitTests.Api;

public class AuthEndpointsTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(ApiTestFixture fx)
    {
        _client = fx.Client;
    }

    [Fact]
    public async Task Register_Then_Login_Succeeds()
    {
        var rnd = Random.Shared.Next(1000, 999999);
        var username = $"player{rnd}";
        var register = new { Username = username, Email = $"{username}@testing.com", Password = "SecureTest123!" };
        var regRes = await _client.PostAsJsonAsync("/api/auth/register", register);
        regRes.StatusCode.Should().Be(HttpStatusCode.OK);

        var login = new { Username = username, Password = "SecureTest123!" };
        var loginRes = await _client.PostAsJsonAsync("/api/auth/login", login);
        loginRes.EnsureSuccessStatusCode();
        var auth = await loginRes.Content.ReadFromJsonAsync<AuthResponse>();
        auth.Should().NotBeNull();
        auth!.UserId.Should().BeGreaterThan(0);
        auth.Username.Should().Be(username);
    }

    private record AuthResponse(int UserId, string Username, string Email, int? CurrentNodeId, int? RealmId, int CargoHolds, DateTime LastLogin, string Token);
}

