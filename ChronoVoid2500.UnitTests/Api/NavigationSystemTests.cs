using System.Net;
using System.Net.Http.Json;
using ChronoVoid2500.UnitTests.TestInfrastructure;
using FluentAssertions;

namespace ChronoVoid2500.UnitTests.Api;

public class NavigationSystemTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public NavigationSystemTests(ApiTestFixture fx)
    {
        _client = fx.Client;
    }

    [Fact]
    public async Task Get_Node_Details_Should_Return_Valid_Information()
    {
        // Get a realm first
        var realmsResponse = await _client.GetAsync("/api/realm");
        realmsResponse.EnsureSuccessStatusCode();
        var realms = await realmsResponse.Content.ReadFromJsonAsync<List<RealmDto>>();
        realms.Should().NotBeNull();
        realms!.Count.Should().BeGreaterThan(0);

        // Get nodes in the realm
        var nodesResponse = await _client.GetAsync($"/api/realm/{realms[0].Id}/nodes");
        nodesResponse.EnsureSuccessStatusCode();
        var nodes = await nodesResponse.Content.ReadFromJsonAsync<List<NodeDto>>();
        nodes.Should().NotBeNull();
        nodes!.Count.Should().BeGreaterThan(0);

        // Get details for the first node
        var nodeId = nodes[0].Id;
        var nodeDetailResponse = await _client.GetAsync($"/api/navigation/node/{nodeId}");
        nodeDetailResponse.EnsureSuccessStatusCode();
        var nodeDetail = await nodeDetailResponse.Content.ReadFromJsonAsync<NodeDetailDto>();
        
        nodeDetail.Should().NotBeNull();
        nodeDetail!.Id.Should().Be(nodeId);
        nodeDetail.NodeNumber.Should().BeGreaterThan(0);
        nodeDetail.RealmName.Should().NotBeNullOrEmpty();
        nodeDetail.StarName.Should().NotBeNullOrEmpty();
        nodeDetail.PlanetCount.Should().BeGreaterThanOrEqualTo(0);
        nodeDetail.ConnectedNodes.Should().NotBeNull();
    }

    [Fact]
    public async Task Move_Between_Connected_Nodes_Should_Succeed()
    {
        // Create a test user
        var username = $"navtest{Random.Shared.Next(1000, 9999)}";
        var registerRequest = new { Username = username, Email = $"{username}@testing.com", Password = "TestPass123!" };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Login to get user info
        var loginRequest = new { Username = username, Password = "TestPass123!" };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var authResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

        // Get realm and nodes
        var realmsResponse = await _client.GetAsync("/api/realm");
        realmsResponse.EnsureSuccessStatusCode();
        var realms = await realmsResponse.Content.ReadFromJsonAsync<List<RealmDto>>();
        var realm = realms![0];

        var nodesResponse = await _client.GetAsync($"/api/realm/{realm.Id}/nodes");
        nodesResponse.EnsureSuccessStatusCode();
        var nodes = await nodesResponse.Content.ReadFromJsonAsync<List<NodeDto>>();
        nodes.Should().NotBeNull();
        nodes!.Count.Should().BeGreaterThan(1, "Need at least 2 nodes to test movement");

        // Find two connected nodes
        var fromNode = nodes[0];
        var toNode = nodes.FirstOrDefault(n => fromNode.ConnectedNodes.Contains(n.Id));
        
        if (toNode != null)
        {
            var moveRequest = new { UserId = authResult!.UserId, FromNodeId = fromNode.Id, ToNodeId = toNode.Id };
            var moveResponse = await _client.PostAsJsonAsync("/api/navigation/move", moveRequest);
            moveResponse.EnsureSuccessStatusCode();

            var moveResult = await moveResponse.Content.ReadFromJsonAsync<NavigationResultDto>();
            moveResult.Should().NotBeNull();
            moveResult!.Success.Should().BeTrue();
            moveResult.ToNodeId.Should().Be(toNode.Id);
        }
    }

    [Fact]
    public async Task Move_To_Unconnected_Node_Should_Fail()
    {
        // Create a test user
        var username = $"navtest{Random.Shared.Next(1000, 9999)}";
        var registerRequest = new { Username = username, Email = $"{username}@testing.com", Password = "TestPass123!" };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Login to get user info
        var loginRequest = new { Username = username, Password = "TestPass123!" };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var authResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

        // Get realm and nodes
        var realmsResponse = await _client.GetAsync("/api/realm");
        realmsResponse.EnsureSuccessStatusCode();
        var realms = await realmsResponse.Content.ReadFromJsonAsync<List<RealmDto>>();
        var realm = realms![0];

        var nodesResponse = await _client.GetAsync($"/api/realm/{realm.Id}/nodes");
        nodesResponse.EnsureSuccessStatusCode();
        var nodes = await nodesResponse.Content.ReadFromJsonAsync<List<NodeDto>>();
        nodes.Should().NotBeNull();
        nodes!.Count.Should().BeGreaterThan(1, "Need at least 2 nodes to test movement");

        // Find two unconnected nodes
        var fromNode = nodes[0];
        var toNode = nodes.FirstOrDefault(n => !fromNode.ConnectedNodes.Contains(n.Id) && n.Id != fromNode.Id);
        
        if (toNode != null)
        {
            var moveRequest = new { UserId = authResult!.UserId, FromNodeId = fromNode.Id, ToNodeId = toNode.Id };
            var moveResponse = await _client.PostAsJsonAsync("/api/navigation/move", moveRequest);
            moveResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }

    [Fact]
    public async Task Get_User_Location_Should_Return_Current_Node()
    {
        // Create a test user
        var username = $"navtest{Random.Shared.Next(1000, 9999)}";
        var registerRequest = new { Username = username, Email = $"{username}@testing.com", Password = "TestPass123!" };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Login to get user info
        var loginRequest = new { Username = username, Password = "TestPass123!" };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var authResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

        // Get user location
        var locationResponse = await _client.GetAsync($"/api/navigation/user/{authResult!.UserId}/location");
        locationResponse.EnsureSuccessStatusCode();
        var location = await locationResponse.Content.ReadFromJsonAsync<UserLocationDto>();
        
        location.Should().NotBeNull();
        location!.UserId.Should().Be(authResult.UserId);
        location.CurrentNodeId.Should().NotBeNull();
        location.RealmId.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_Users_In_Realm_Should_Return_User_List()
    {
        // Get a realm first
        var realmsResponse = await _client.GetAsync("/api/realm");
        realmsResponse.EnsureSuccessStatusCode();
        var realms = await realmsResponse.Content.ReadFromJsonAsync<List<RealmDto>>();
        realms.Should().NotBeNull();
        realms!.Count.Should().BeGreaterThan(0);

        var realmId = realms[0].Id;

        // Get users in the realm
        var usersResponse = await _client.GetAsync($"/api/navigation/realm/{realmId}/users");
        usersResponse.EnsureSuccessStatusCode();
        var users = await usersResponse.Content.ReadFromJsonAsync<List<UserLocationDto>>();
        
        users.Should().NotBeNull();
        // Note: This might be empty if no users are in the realm, which is valid
    }

    // DTOs for the tests
    private record AuthResponse(int UserId, string Username, string Email, int? CurrentNodeId, int? RealmId, int CargoHolds, DateTime LastLogin, string Token);
    private record RealmDto(int Id, string Name, int NodeCount, int QuantumStationSeedRate, bool NoDeadNodes, DateTime CreatedAt, bool IsActive);
    private record NodeDto(int Id, int NodeNumber, int CoordinateX, int CoordinateY, bool HasQuantumStation, List<int> ConnectedNodes);
    private record NodeDetailDto(int Id, int NodeNumber, string RealmName, bool HasQuantumStation, string StarName, int PlanetCount, List<ConnectedNodeDto> ConnectedNodes);
    private record ConnectedNodeDto(int NodeId, int NodeNumber, bool HasQuantumStation, string StarName, int PlanetCount);
    private record NavigationResultDto(bool Success, int FromNodeId, int ToNodeId, string Message);
    private record UserLocationDto(int UserId, string Username, int? CurrentNodeId, int? RealmId, DateTime LastLogin);
}
