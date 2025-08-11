using System.Net;
using System.Net.Http.Json;
using ChronoVoid2500.UnitTests.TestInfrastructure;
using FluentAssertions;

namespace ChronoVoid2500.UnitTests.Api;

public class PlanetSystemTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public PlanetSystemTests(ApiTestFixture fx)
    {
        _client = fx.Client;
    }

    [Fact]
    public async Task Get_Planets_Should_Return_List_Of_Planets()
    {
        var response = await _client.GetAsync("/api/planet");
        response.EnsureSuccessStatusCode();
        
        var planets = await response.Content.ReadFromJsonAsync<List<PlanetDto>>();
        planets.Should().NotBeNull();
        planets!.Count.Should().BeGreaterThanOrEqualTo(0);
        
        if (planets.Count > 0)
        {
            var planet = planets[0];
            planet.Id.Should().BeGreaterThan(0);
            planet.Name.Should().NotBeNullOrEmpty();
            planet.PlanetType.Should().NotBeNullOrEmpty();
            planet.Size.Should().BeGreaterThan(0);
            planet.Distance.Should().BeGreaterThanOrEqualTo(0);
            planet.OwnerId.Should().BeOneOf(null, 0); // Can be unowned
        }
    }

    [Fact]
    public async Task Get_Planet_By_Id_Should_Return_Valid_Planet()
    {
        // Get a list of planets first
        var planetsResponse = await _client.GetAsync("/api/planet");
        planetsResponse.EnsureSuccessStatusCode();
        var planets = await planetsResponse.Content.ReadFromJsonAsync<List<PlanetDto>>();
        planets.Should().NotBeNull();
        planets!.Count.Should().BeGreaterThan(0);

        var planetId = planets[0].Id;
        var response = await _client.GetAsync($"/api/planet/{planetId}");
        response.EnsureSuccessStatusCode();
        
        var planet = await response.Content.ReadFromJsonAsync<PlanetDto>();
        planet.Should().NotBeNull();
        planet!.Id.Should().Be(planetId);
        planet.Name.Should().NotBeNullOrEmpty();
        planet.PlanetType.Should().NotBeNullOrEmpty();
        planet.Size.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Get_Planets_By_Node_Should_Return_Node_Planets()
    {
        // Get a realm and node first
        var realmsResponse = await _client.GetAsync("/api/realm");
        realmsResponse.EnsureSuccessStatusCode();
        var realms = await realmsResponse.Content.ReadFromJsonAsync<List<RealmDto>>();
        realms.Should().NotBeNull();
        realms!.Count.Should().BeGreaterThan(0);

        var nodesResponse = await _client.GetAsync($"/api/realm/{realms[0].Id}/nodes");
        nodesResponse.EnsureSuccessStatusCode();
        var nodes = await nodesResponse.Content.ReadFromJsonAsync<List<NodeDto>>();
        nodes.Should().NotBeNull();
        nodes!.Count.Should().BeGreaterThan(0);

        var nodeId = nodes[0].Id;
        var response = await _client.GetAsync($"/api/planet/node/{nodeId}");
        response.EnsureSuccessStatusCode();
        
        var planets = await response.Content.ReadFromJsonAsync<List<PlanetDto>>();
        planets.Should().NotBeNull();
        // Note: Nodes might not have planets yet, which is valid
    }

    [Fact]
    public async Task Get_User_Planets_Should_Return_Owned_Planets()
    {
        // Create a test user first
        var username = $"planettest{Random.Shared.Next(1000, 9999)}";
        var registerRequest = new { Username = username, Email = $"{username}@testing.com", Password = "TestPass123!" };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Login to get user info
        var loginRequest = new { Username = username, Password = "TestPass123!" };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var authResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

        // Get user planets
        var response = await _client.GetAsync($"/api/planet/user/{authResult!.UserId}");
        response.EnsureSuccessStatusCode();
        
        var planets = await response.Content.ReadFromJsonAsync<List<PlanetDto>>();
        planets.Should().NotBeNull();
        // Note: New users might not own planets yet, which is valid
    }

    [Fact]
    public async Task Purchase_Planet_Should_Transfer_Ownership()
    {
        // Create a test user first
        var username = $"planettest{Random.Shared.Next(1000, 9999)}";
        var registerRequest = new { Username = username, Email = $"{username}@testing.com", Password = "TestPass123!" };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginRequest = new { Username = username, Password = "TestPass123!" };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var authResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

        // Get an unowned planet
        var planetsResponse = await _client.GetAsync("/api/planet");
        planetsResponse.EnsureSuccessStatusCode();
        var planets = await planetsResponse.Content.ReadFromJsonAsync<List<PlanetDto>>();
        planets.Should().NotBeNull();
        planets!.Count.Should().BeGreaterThan(0);

        var unownedPlanet = planets.FirstOrDefault(p => p.OwnerId == null || p.OwnerId == 0);
        unownedPlanet.Should().NotBeNull("Should have at least one unowned planet");

        // Purchase the planet
        var purchaseRequest = new PlanetPurchaseDto
        {
            UserId = authResult!.UserId,
            PlanetId = unownedPlanet!.Id,
            PurchaseType = "Credits",
            Amount = 10000
        };

        var response = await _client.PostAsJsonAsync("/api/planet/purchase", purchaseRequest);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<PlanetPurchaseResultDto>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Message.Should().Contain("purchased successfully");

        // Verify ownership transfer
        var planetResponse = await _client.GetAsync($"/api/planet/{unownedPlanet.Id}");
        planetResponse.EnsureSuccessStatusCode();
        var updatedPlanet = await planetResponse.Content.ReadFromJsonAsync<PlanetDto>();
        
        updatedPlanet.Should().NotBeNull();
        updatedPlanet!.OwnerId.Should().Be(authResult.UserId);
    }

    [Fact]
    public async Task Purchase_Already_Owned_Planet_Should_Fail()
    {
        // Create two test users
        var user1Name = $"planet1{Random.Shared.Next(1000, 9999)}";
        var user2Name = $"planet2{Random.Shared.Next(1000, 9999)}";
        
        var register1Request = new { Username = user1Name, Email = $"{user1Name}@testing.com", Password = "TestPass123!" };
        var register1Response = await _client.PostAsJsonAsync("/api/auth/register", register1Request);
        register1Response.StatusCode.Should().Be(HttpStatusCode.OK);

        var register2Request = new { Username = user2Name, Email = $"{user2Name}@testing.com", Password = "TestPass123!" };
        var register2Response = await _client.PostAsJsonAsync("/api/auth/register", register2Request);
        register2Response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Login to get user info
        var login1Request = new { Username = user1Name, Password = "TestPass123!" };
        var login1Response = await _client.PostAsJsonAsync("/api/auth/login", login1Request);
        login1Response.EnsureSuccessStatusCode();
        var auth1Result = await login1Response.Content.ReadFromJsonAsync<AuthResponse>();

        var login2Request = new { Username = user2Name, Password = "TestPass123!" };
        var login2Response = await _client.PostAsJsonAsync("/api/auth/login", login2Request);
        login2Response.EnsureSuccessStatusCode();
        var auth2Result = await login2Response.Content.ReadFromJsonAsync<AuthResponse>();

        // Get an unowned planet
        var planetsResponse = await _client.GetAsync("/api/planet");
        planetsResponse.EnsureSuccessStatusCode();
        var planets = await planetsResponse.Content.ReadFromJsonAsync<List<PlanetDto>>();
        planets.Should().NotBeNull();
        planets!.Count.Should().BeGreaterThan(0);

        var unownedPlanet = planets.FirstOrDefault(p => p.OwnerId == null || p.OwnerId == 0);
        unownedPlanet.Should().NotBeNull("Should have at least one unowned planet");

        // First user purchases the planet
        var purchase1Request = new PlanetPurchaseDto
        {
            UserId = auth1Result!.UserId,
            PlanetId = unownedPlanet!.Id,
            PurchaseType = "Credits",
            Amount = 10000
        };

        var purchase1Response = await _client.PostAsJsonAsync("/api/planet/purchase", purchase1Request);
        purchase1Response.EnsureSuccessStatusCode();

        // Second user tries to purchase the same planet
        var purchase2Request = new PlanetPurchaseDto
        {
            UserId = auth2Result!.UserId,
            PlanetId = unownedPlanet.Id,
            PurchaseType = "Credits",
            Amount = 10000
        };

        var purchase2Response = await _client.PostAsJsonAsync("/api/planet/purchase", purchase2Request);
        purchase2Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_Planet_Resources_Should_Return_Resource_Production()
    {
        // Get a planet first
        var planetsResponse = await _client.GetAsync("/api/planet");
        planetsResponse.EnsureSuccessStatusCode();
        var planets = await planetsResponse.Content.ReadFromJsonAsync<List<PlanetDto>>();
        planets.Should().NotBeNull();
        planets!.Count.Should().BeGreaterThan(0);

        var planetId = planets[0].Id;
        var response = await _client.GetAsync($"/api/planet/{planetId}/resources");
        response.EnsureSuccessStatusCode();
        
        var resources = await response.Content.ReadFromJsonAsync<PlanetResourcesDto>();
        resources.Should().NotBeNull();
        resources!.PlanetId.Should().Be(planetId);
        resources.ResourceType.Should().NotBeNullOrEmpty();
        resources.ProductionRate.Should().BeGreaterThanOrEqualTo(0);
        resources.CurrentStock.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task Collect_Planet_Resources_Should_Transfer_To_User()
    {
        // Create a test user and give them a planet
        var username = $"collect{Random.Shared.Next(1000, 9999)}";
        var registerRequest = new { Username = username, Email = $"{username}@testing.com", Password = "TestPass123!" };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginRequest = new { Username = username, Password = "TestPass123!" };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var authResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

        // Get an unowned planet
        var planetsResponse = await _client.GetAsync("/api/planet");
        planetsResponse.EnsureSuccessStatusCode();
        var planets = await planetsResponse.Content.ReadFromJsonAsync<List<PlanetDto>>();
        planets.Should().NotBeNull();
        planets!.Count.Should().BeGreaterThan(0);

        var unownedPlanet = planets.FirstOrDefault(p => p.OwnerId == null || p.OwnerId == 0);
        unownedPlanet.Should().NotBeNull("Should have at least one unowned planet");

        // Purchase the planet
        var purchaseRequest = new PlanetPurchaseDto
        {
            UserId = authResult!.UserId,
            PlanetId = unownedPlanet!.Id,
            PurchaseType = "Credits",
            Amount = 10000
        };

        var purchaseResponse = await _client.PostAsJsonAsync("/api/planet/purchase", purchaseRequest);
        purchaseResponse.EnsureSuccessStatusCode();

        // Collect resources
        var collectRequest = new ResourceCollectionDto
        {
            UserId = authResult.UserId,
            PlanetId = unownedPlanet.Id
        };

        var response = await _client.PostAsJsonAsync("/api/planet/collect-resources", collectRequest);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<ResourceCollectionResultDto>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.CollectedAmount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task Collect_Resources_From_Unowned_Planet_Should_Fail()
    {
        // Create a test user
        var username = $"collect{Random.Shared.Next(1000, 9999)}";
        var registerRequest = new { Username = username, Email = $"{username}@testing.com", Password = "TestPass123!" };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginRequest = new { Username = username, Password = "TestPass123!" };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var authResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

        // Get an unowned planet
        var planetsResponse = await _client.GetAsync("/api/planet");
        planetsResponse.EnsureSuccessStatusCode();
        var planets = await planetsResponse.Content.ReadFromJsonAsync<List<PlanetDto>>();
        planets.Should().NotBeNull();
        planets!.Count.Should().BeGreaterThan(0);

        var unownedPlanet = planets.FirstOrDefault(p => p.OwnerId == null || p.OwnerId == 0);
        unownedPlanet.Should().NotBeNull("Should have at least one unowned planet");

        // Try to collect resources from unowned planet
        var collectRequest = new ResourceCollectionDto
        {
            UserId = authResult!.UserId,
            PlanetId = unownedPlanet!.Id
        };

        var response = await _client.PostAsJsonAsync("/api/planet/collect-resources", collectRequest);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // DTOs for the tests
    private record AuthResponse(int UserId, string Username, string Email, int? CurrentNodeId, int? RealmId, int CargoHolds, DateTime LastLogin, string Token);
    private record RealmDto(int Id, string Name, int NodeCount, int QuantumStationSeedRate, bool NoDeadNodes, DateTime CreatedAt, bool IsActive);
    private record NodeDto(int Id, int NodeNumber, int CoordinateX, int CoordinateY, bool HasQuantumStation, List<int> ConnectedNodes);
    private record PlanetDto(int Id, string Name, string PlanetType, int Size, int Distance, int? OwnerId, int NodeId);
    private record PlanetPurchaseDto(int UserId, int PlanetId, string PurchaseType, int Amount);
    private record PlanetPurchaseResultDto(bool Success, string Message);
    private record PlanetResourcesDto(int PlanetId, string ResourceType, int ProductionRate, int CurrentStock);
    private record ResourceCollectionDto(int UserId, int PlanetId);
    private record ResourceCollectionResultDto(bool Success, int CollectedAmount, string Message);
}
