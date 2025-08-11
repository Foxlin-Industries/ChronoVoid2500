using System.Net;
using System.Net.Http.Json;
using ChronoVoid2500.UnitTests.TestInfrastructure;
using FluentAssertions;

namespace ChronoVoid2500.UnitTests.Api;

public class AdminSystemTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public AdminSystemTests(ApiTestFixture fx)
    {
        _client = fx.Client;
    }

    [Fact]
    public async Task Get_System_Statistics_Should_Return_Valid_Data()
    {
        var response = await _client.GetAsync("/api/admin/statistics");
        response.EnsureSuccessStatusCode();
        
        var stats = await response.Content.ReadFromJsonAsync<SystemStatisticsDto>();
        stats.Should().NotBeNull();
        stats!.TotalUsers.Should().BeGreaterThanOrEqualTo(0);
        stats.TotalRealms.Should().BeGreaterThanOrEqualTo(0);
        stats.TotalPlanets.Should().BeGreaterThanOrEqualTo(0);
        stats.TotalShips.Should().BeGreaterThanOrEqualTo(0);
        stats.ActiveUsers.Should().BeGreaterThanOrEqualTo(0);
        stats.SystemUptime.Should().BeGreaterThan(TimeSpan.Zero);
    }

    [Fact]
    public async Task Get_All_Users_Should_Return_User_List()
    {
        var response = await _client.GetAsync("/api/admin/users");
        response.EnsureSuccessStatusCode();
        
        var users = await response.Content.ReadFromJsonAsync<List<AdminUserDto>>();
        users.Should().NotBeNull();
        users!.Count.Should().BeGreaterThanOrEqualTo(0);
        
        if (users.Count > 0)
        {
            var user = users[0];
            user.Id.Should().BeGreaterThan(0);
            user.Username.Should().NotBeNullOrEmpty();
            user.Email.Should().NotBeNullOrEmpty();
            user.CreatedAt.Should().BeBefore(DateTime.UtcNow.AddMinutes(1));
            user.LastLogin.Should().BeBefore(DateTime.UtcNow.AddMinutes(1));
        }
    }

    [Fact]
    public async Task Get_User_Details_Should_Return_Complete_User_Info()
    {
        // Create a test user first
        var username = $"admintest{Random.Shared.Next(1000, 9999)}";
        var registerRequest = new { Username = username, Email = $"{username}@testing.com", Password = "TestPass123!" };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Login to get user info
        var loginRequest = new { Username = username, Password = "TestPass123!" };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var authResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

        // Get user details as admin
        var response = await _client.GetAsync($"/api/admin/users/{authResult!.UserId}");
        response.EnsureSuccessStatusCode();
        
        var userDetails = await response.Content.ReadFromJsonAsync<AdminUserDetailDto>();
        userDetails.Should().NotBeNull();
        userDetails!.Id.Should().Be(authResult.UserId);
        userDetails.Username.Should().Be(username);
        userDetails.Email.Should().Be($"{username}@testing.com");
        userDetails.CreatedAt.Should().BeBefore(DateTime.UtcNow.AddMinutes(1));
        userDetails.LastLogin.Should().BeBefore(DateTime.UtcNow.AddMinutes(1));
        userDetails.CurrentNodeId.Should().NotBeNull();
        userDetails.RealmId.Should().NotBeNull();
        userDetails.CargoHolds.Should().Be(300); // Default value
    }

    [Fact]
    public async Task Update_User_Should_Modify_User_Properties()
    {
        // Create a test user first
        var username = $"admintest{Random.Shared.Next(1000, 9999)}";
        var registerRequest = new { Username = username, Email = $"{username}@testing.com", Password = "TestPass123!" };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginRequest = new { Username = username, Password = "TestPass123!" };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var authResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

        // Update user as admin
        var updateRequest = new AdminUserUpdateDto
        {
            Username = "UpdatedUsername",
            Email = "updated@testing.com",
            CargoHolds = 500
        };

        var response = await _client.PutAsJsonAsync($"/api/admin/users/{authResult!.UserId}", updateRequest);
        response.EnsureSuccessStatusCode();

        // Verify the update
        var userResponse = await _client.GetAsync($"/api/admin/users/{authResult.UserId}");
        userResponse.EnsureSuccessStatusCode();
        var updatedUser = await userResponse.Content.ReadFromJsonAsync<AdminUserDetailDto>();
        
        updatedUser.Should().NotBeNull();
        updatedUser!.Username.Should().Be("UpdatedUsername");
        updatedUser.Email.Should().Be("updated@testing.com");
        updatedUser.CargoHolds.Should().Be(500);
    }

    [Fact]
    public async Task Delete_User_Should_Remove_User_From_System()
    {
        // Create a test user first
        var username = $"admintest{Random.Shared.Next(1000, 9999)}";
        var registerRequest = new { Username = username, Email = $"{username}@testing.com", Password = "TestPass123!" };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginRequest = new { Username = username, Password = "TestPass123!" };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var authResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

        // Delete user as admin
        var response = await _client.DeleteAsync($"/api/admin/users/{authResult!.UserId}");
        response.EnsureSuccessStatusCode();

        // Verify the user is deleted
        var userResponse = await _client.GetAsync($"/api/admin/users/{authResult.UserId}");
        userResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_Realm_Administration_Should_Return_Realm_List()
    {
        var response = await _client.GetAsync("/api/admin/realms");
        response.EnsureSuccessStatusCode();
        
        var realms = await response.Content.ReadFromJsonAsync<List<AdminRealmDto>>();
        realms.Should().NotBeNull();
        realms!.Count.Should().BeGreaterThanOrEqualTo(0);
        
        if (realms.Count > 0)
        {
            var realm = realms[0];
            realm.Id.Should().BeGreaterThan(0);
            realm.Name.Should().NotBeNullOrEmpty();
            realm.NodeCount.Should().BeGreaterThan(0);
            realm.UserCount.Should().BeGreaterThanOrEqualTo(0);
            realm.CreatedAt.Should().BeBefore(DateTime.UtcNow.AddMinutes(1));
            realm.IsActive.Should().BeTrue();
        }
    }

    [Fact]
    public async Task Get_Realm_Details_Should_Return_Complete_Realm_Info()
    {
        // Get a realm first
        var realmsResponse = await _client.GetAsync("/api/admin/realms");
        realmsResponse.EnsureSuccessStatusCode();
        var realms = await realmsResponse.Content.ReadFromJsonAsync<List<AdminRealmDto>>();
        realms.Should().NotBeNull();
        realms!.Count.Should().BeGreaterThan(0);

        var realmId = realms[0].Id;
        var response = await _client.GetAsync($"/api/admin/realms/{realmId}");
        response.EnsureSuccessStatusCode();
        
        var realmDetails = await response.Content.ReadFromJsonAsync<AdminRealmDetailDto>();
        realmDetails.Should().NotBeNull();
        realmDetails!.Id.Should().Be(realmId);
        realmDetails.Name.Should().NotBeNullOrEmpty();
        realmDetails.NodeCount.Should().BeGreaterThan(0);
        realmDetails.UserCount.Should().BeGreaterThanOrEqualTo(0);
        realmDetails.CreatedAt.Should().BeBefore(DateTime.UtcNow.AddMinutes(1));
        realmDetails.IsActive.Should().BeTrue();
        realmDetails.Users.Should().NotBeNull();
        realmDetails.Nodes.Should().NotBeNull();
    }

    [Fact]
    public async Task Deactivate_Realm_Should_Change_Status()
    {
        // Get an active realm first
        var realmsResponse = await _client.GetAsync("/api/admin/realms");
        realmsResponse.EnsureSuccessStatusCode();
        var realms = await realmsResponse.Content.ReadFromJsonAsync<List<AdminRealmDto>>();
        realms.Should().NotBeNull();
        realms!.Count.Should().BeGreaterThan(0);

        var activeRealm = realms.FirstOrDefault(r => r.IsActive);
        activeRealm.Should().NotBeNull("Should have at least one active realm");

        // Deactivate the realm
        var response = await _client.PostAsJsonAsync($"/api/admin/realms/{activeRealm!.Id}/deactivate", new {});
        response.EnsureSuccessStatusCode();

        // Verify the realm is deactivated
        var realmResponse = await _client.GetAsync($"/api/admin/realms/{activeRealm.Id}");
        realmResponse.EnsureSuccessStatusCode();
        var updatedRealm = await realmResponse.Content.ReadFromJsonAsync<AdminRealmDetailDto>();
        
        updatedRealm.Should().NotBeNull();
        updatedRealm!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Get_System_Logs_Should_Return_Log_Entries()
    {
        var response = await _client.GetAsync("/api/admin/logs");
        response.EnsureSuccessStatusCode();
        
        var logs = await response.Content.ReadFromJsonAsync<List<SystemLogDto>>();
        logs.Should().NotBeNull();
        logs!.Count.Should().BeGreaterThanOrEqualTo(0);
        
        if (logs.Count > 0)
        {
            var log = logs[0];
            log.Id.Should().BeGreaterThan(0);
            log.Level.Should().BeOneOf("Info", "Warning", "Error", "Debug");
            log.Message.Should().NotBeNullOrEmpty();
            log.Timestamp.Should().BeBefore(DateTime.UtcNow.AddMinutes(1));
        }
    }

    [Fact]
    public async Task Get_System_Logs_With_Filter_Should_Return_Filtered_Results()
    {
        var response = await _client.GetAsync("/api/admin/logs?level=Error&limit=10");
        response.EnsureSuccessStatusCode();
        
        var logs = await response.Content.ReadFromJsonAsync<List<SystemLogDto>>();
        logs.Should().NotBeNull();
        
        // All returned logs should be Error level
        foreach (var log in logs!)
        {
            log.Level.Should().Be("Error");
        }
    }

    [Fact]
    public async Task Get_User_Activity_Should_Return_Activity_Log()
    {
        // Create a test user first
        var username = $"activity{Random.Shared.Next(1000, 9999)}";
        var registerRequest = new { Username = username, Email = $"{username}@testing.com", Password = "TestPass123!" };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginRequest = new { Username = username, Password = "TestPass123!" };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var authResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

        // Get user activity
        var response = await _client.GetAsync($"/api/admin/users/{authResult!.UserId}/activity");
        response.EnsureSuccessStatusCode();
        
        var activity = await response.Content.ReadFromJsonAsync<List<UserActivityDto>>();
        activity.Should().NotBeNull();
        // Note: New users might not have much activity yet, which is valid
    }

    [Fact]
    public async Task Get_System_Health_Should_Return_Health_Status()
    {
        var response = await _client.GetAsync("/api/admin/health");
        response.EnsureSuccessStatusCode();
        
        var health = await response.Content.ReadFromJsonAsync<SystemHealthDto>();
        health.Should().NotBeNull();
        health!.Status.Should().BeOneOf("Healthy", "Degraded", "Unhealthy");
        health.DatabaseStatus.Should().Be("Connected");
        health.ApiStatus.Should().Be("Running");
        health.ResponseTime.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Get_Non_Existent_User_Should_Return_NotFound()
    {
        var response = await _client.GetAsync("/api/admin/users/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_Non_Existent_Realm_Should_Return_NotFound()
    {
        var response = await _client.GetAsync("/api/admin/realms/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // DTOs for the tests
    private record AuthResponse(int UserId, string Username, string Email, int? CurrentNodeId, int? RealmId, int CargoHolds, DateTime LastLogin, string Token);
    private record SystemStatisticsDto(int TotalUsers, int TotalRealms, int TotalPlanets, int TotalShips, int ActiveUsers, TimeSpan SystemUptime);
    private record AdminUserDto(int Id, string Username, string Email, DateTime CreatedAt, DateTime LastLogin, bool IsActive);
    private record AdminUserDetailDto(int Id, string Username, string Email, DateTime CreatedAt, DateTime LastLogin, int? CurrentNodeId, int? RealmId, int CargoHolds, bool IsActive);
    private record AdminUserUpdateDto(string Username, string Email, int CargoHolds);
    private record AdminRealmDto(int Id, string Name, int NodeCount, int UserCount, DateTime CreatedAt, bool IsActive);
    private record AdminRealmDetailDto(int Id, string Name, int NodeCount, int UserCount, DateTime CreatedAt, bool IsActive, List<AdminUserDto> Users, List<AdminNodeDto> Nodes);
    private record AdminNodeDto(int Id, int NodeNumber, bool HasQuantumStation, int PlanetCount);
    private record SystemLogDto(int Id, string Level, string Message, DateTime Timestamp, string? UserId);
    private record UserActivityDto(int Id, string Action, DateTime Timestamp, string Details);
    private record SystemHealthDto(string Status, string DatabaseStatus, string ApiStatus, int ResponseTime);
}
