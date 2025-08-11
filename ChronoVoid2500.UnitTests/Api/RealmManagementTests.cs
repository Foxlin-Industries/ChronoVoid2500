using System.Net;
using System.Net.Http.Json;
using ChronoVoid2500.UnitTests.TestInfrastructure;
using FluentAssertions;

namespace ChronoVoid2500.UnitTests.Api;

public class RealmManagementTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public RealmManagementTests(ApiTestFixture fx)
    {
        _client = fx.Client;
    }

    [Fact]
    public async Task Get_Realms_Should_Return_List_Of_Realms()
    {
        var response = await _client.GetAsync("/api/realm");
        response.EnsureSuccessStatusCode();
        
        var realms = await response.Content.ReadFromJsonAsync<List<RealmDto>>();
        realms.Should().NotBeNull();
        realms!.Count.Should().BeGreaterThanOrEqualTo(0);
        
        if (realms.Count > 0)
        {
            var realm = realms[0];
            realm.Id.Should().BeGreaterThan(0);
            realm.Name.Should().NotBeNullOrEmpty();
            realm.NodeCount.Should().BeGreaterThan(0);
            realm.QuantumStationSeedRate.Should().BeGreaterThanOrEqualTo(0);
            realm.CreatedAt.Should().BeBefore(DateTime.UtcNow.AddMinutes(1));
        }
    }

    [Fact]
    public async Task Create_Realm_Should_Succeed_With_Valid_Parameters()
    {
        var realmName = $"TestRealm{Random.Shared.Next(1000, 9999)}";
        var createRequest = new RealmCreationDto
        {
            Name = realmName,
            NodeCount = 10,
            QuantumStationSeedRate = 30,
            NoDeadNodes = false
        };

        var response = await _client.PostAsJsonAsync("/api/realm/create", createRequest);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<RealmCreationResultDto>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.RealmId.Should().BeGreaterThan(0);
        result.Message.Should().Contain("created successfully");
    }

    [Fact]
    public async Task Create_Realm_Should_Fail_With_Invalid_Parameters()
    {
        var createRequest = new RealmCreationDto
        {
            Name = "", // Invalid empty name
            NodeCount = 0, // Invalid node count
            QuantumStationSeedRate = 150, // Invalid percentage
            NoDeadNodes = false
        };

        var response = await _client.PostAsJsonAsync("/api/realm/create", createRequest);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_Realm_Nodes_Should_Return_Valid_Node_List()
    {
        // Get a realm first
        var realmsResponse = await _client.GetAsync("/api/realm");
        realmsResponse.EnsureSuccessStatusCode();
        var realms = await realmsResponse.Content.ReadFromJsonAsync<List<RealmDto>>();
        realms.Should().NotBeNull();
        realms!.Count.Should().BeGreaterThan(0);

        var realmId = realms[0].Id;
        var nodesResponse = await _client.GetAsync($"/api/realm/{realmId}/nodes");
        nodesResponse.EnsureSuccessStatusCode();
        
        var nodes = await nodesResponse.Content.ReadFromJsonAsync<List<NodeDto>>();
        nodes.Should().NotBeNull();
        nodes!.Count.Should().Be(realms[0].NodeCount);
        
        foreach (var node in nodes)
        {
            node.Id.Should().BeGreaterThan(0);
            node.NodeNumber.Should().BeGreaterThan(0);
            node.CoordinateX.Should().BeGreaterThanOrEqualTo(0);
            node.CoordinateY.Should().BeGreaterThanOrEqualTo(0);
            node.ConnectedNodes.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task Get_Realm_Details_Should_Return_Complete_Information()
    {
        // Get a realm first
        var realmsResponse = await _client.GetAsync("/api/realm");
        realmsResponse.EnsureSuccessStatusCode();
        var realms = await realmsResponse.Content.ReadFromJsonAsync<List<RealmDto>>();
        realms.Should().NotBeNull();
        realms!.Count.Should().BeGreaterThan(0);

        var realmId = realms[0].Id;
        var detailsResponse = await _client.GetAsync($"/api/realm/{realmId}");
        detailsResponse.EnsureSuccessStatusCode();
        
        var details = await detailsResponse.Content.ReadFromJsonAsync<RealmDetailDto>();
        details.Should().NotBeNull();
        details!.Id.Should().Be(realmId);
        details.Name.Should().NotBeNullOrEmpty();
        details.NodeCount.Should().BeGreaterThan(0);
        details.QuantumStationSeedRate.Should().BeGreaterThanOrEqualTo(0);
        details.CreatedAt.Should().BeBefore(DateTime.UtcNow.AddMinutes(1));
        details.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Get_Non_Existent_Realm_Should_Return_NotFound()
    {
        var response = await _client.GetAsync("/api/realm/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_Non_Existent_Realm_Nodes_Should_Return_NotFound()
    {
        var response = await _client.GetAsync("/api/realm/99999/nodes");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_Realm_With_No_Dead_Nodes_Should_Generate_Connected_Network()
    {
        var realmName = $"ConnectedRealm{Random.Shared.Next(1000, 9999)}";
        var createRequest = new RealmCreationDto
        {
            Name = realmName,
            NodeCount = 5,
            QuantumStationSeedRate = 20,
            NoDeadNodes = true
        };

        var createResponse = await _client.PostAsJsonAsync("/api/realm/create", createRequest);
        createResponse.EnsureSuccessStatusCode();
        
        var result = await createResponse.Content.ReadFromJsonAsync<RealmCreationResultDto>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();

        // Get the nodes and verify connectivity
        var nodesResponse = await _client.GetAsync($"/api/realm/{result.RealmId}/nodes");
        nodesResponse.EnsureSuccessStatusCode();
        
        var nodes = await nodesResponse.Content.ReadFromJsonAsync<List<NodeDto>>();
        nodes.Should().NotBeNull();
        nodes!.Count.Should().Be(5);

        // Verify that all nodes have at least one connection
        foreach (var node in nodes)
        {
            node.ConnectedNodes.Should().NotBeEmpty("All nodes should be connected in a no-dead-nodes realm");
        }
    }

    [Fact]
    public async Task Create_Realm_With_Quantum_Stations_Should_Generate_Starbases()
    {
        var realmName = $"QuantumRealm{Random.Shared.Next(1000, 9999)}";
        var createRequest = new RealmCreationDto
        {
            Name = realmName,
            NodeCount = 10,
            QuantumStationSeedRate = 50, // 50% should have stations
            NoDeadNodes = false
        };

        var createResponse = await _client.PostAsJsonAsync("/api/realm/create", createRequest);
        createResponse.EnsureSuccessStatusCode();
        
        var result = await createResponse.Content.ReadFromJsonAsync<RealmCreationResultDto>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();

        // Get the nodes and verify quantum stations
        var nodesResponse = await _client.GetAsync($"/api/realm/{result.RealmId}/nodes");
        nodesResponse.EnsureSuccessStatusCode();
        
        var nodes = await nodesResponse.Content.ReadFromJsonAsync<List<NodeDto>>();
        nodes.Should().NotBeNull();
        nodes!.Count.Should().Be(10);

        // Count nodes with quantum stations
        var nodesWithStations = nodes.Count(n => n.HasQuantumStation);
        nodesWithStations.Should().BeGreaterThan(0, "At least some nodes should have quantum stations");
        
        // With 50% seed rate, we should have approximately 5 stations (allowing for randomness)
        nodesWithStations.Should().BeInRange(2, 8, "With 50% seed rate, should have reasonable number of stations");
    }

    // DTOs for the tests
    private record RealmDto(int Id, string Name, int NodeCount, int QuantumStationSeedRate, bool NoDeadNodes, DateTime CreatedAt, bool IsActive);
    private record NodeDto(int Id, int NodeNumber, int CoordinateX, int CoordinateY, bool HasQuantumStation, List<int> ConnectedNodes);
    private record RealmCreationDto
    {
        public string Name { get; init; } = string.Empty;
        public int NodeCount { get; init; }
        public int QuantumStationSeedRate { get; init; }
        public bool NoDeadNodes { get; init; }
    }
    private record RealmCreationResultDto(bool Success, int RealmId, string Message);
    private record RealmDetailDto(int Id, string Name, int NodeCount, int QuantumStationSeedRate, bool NoDeadNodes, DateTime CreatedAt, bool IsActive);
}
