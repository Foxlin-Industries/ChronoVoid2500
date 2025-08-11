using System.Net;
using System.Net.Http.Json;
using ChronoVoid2500.UnitTests.TestInfrastructure;
using FluentAssertions;

namespace ChronoVoid2500.UnitTests.Api;

public class ShipAndCombatTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public ShipAndCombatTests(ApiTestFixture fx)
    {
        _client = fx.Client;
    }

    [Fact]
    public async Task Get_Ships_Should_Return_List_Of_Ships()
    {
        var response = await _client.GetAsync("/api/ship");
        response.EnsureSuccessStatusCode();
        
        var ships = await response.Content.ReadFromJsonAsync<List<ShipDto>>();
        ships.Should().NotBeNull();
        ships!.Count.Should().BeGreaterThanOrEqualTo(0);
        
        if (ships.Count > 0)
        {
            var ship = ships[0];
            ship.Id.Should().BeGreaterThan(0);
            ship.Name.Should().NotBeNullOrEmpty();
            ship.ShipType.Should().NotBeNullOrEmpty();
            ship.CargoCapacity.Should().BeGreaterThan(0);
            ship.WeaponLevel.Should().BeGreaterThanOrEqualTo(0);
            ship.ShieldLevel.Should().BeGreaterThanOrEqualTo(0);
        }
    }

    [Fact]
    public async Task Get_Ship_By_Id_Should_Return_Valid_Ship()
    {
        // Get a list of ships first
        var shipsResponse = await _client.GetAsync("/api/ship");
        shipsResponse.EnsureSuccessStatusCode();
        var ships = await shipsResponse.Content.ReadFromJsonAsync<List<ShipDto>>();
        ships.Should().NotBeNull();
        ships!.Count.Should().BeGreaterThan(0);

        var shipId = ships[0].Id;
        var response = await _client.GetAsync($"/api/ship/{shipId}");
        response.EnsureSuccessStatusCode();
        
        var ship = await response.Content.ReadFromJsonAsync<ShipDto>();
        ship.Should().NotBeNull();
        ship!.Id.Should().Be(shipId);
        ship.Name.Should().NotBeNullOrEmpty();
        ship.ShipType.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Get_User_Ships_Should_Return_User_Ships()
    {
        // Create a test user first
        var username = $"shiptest{Random.Shared.Next(1000, 9999)}";
        var registerRequest = new { Username = username, Email = $"{username}@testing.com", Password = "TestPass123!" };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Login to get user info
        var loginRequest = new { Username = username, Password = "TestPass123!" };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var authResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

        // Get user ships
        var response = await _client.GetAsync($"/api/ship/user/{authResult!.UserId}");
        response.EnsureSuccessStatusCode();
        
        var ships = await response.Content.ReadFromJsonAsync<List<ShipDto>>();
        ships.Should().NotBeNull();
        // Note: New users might not have ships yet, which is valid
    }

    [Fact]
    public async Task Purchase_Ship_Should_Create_New_Ship_For_User()
    {
        // Create a test user first
        var username = $"shiptest{Random.Shared.Next(1000, 9999)}";
        var registerRequest = new { Username = username, Email = $"{username}@testing.com", Password = "TestPass123!" };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Login to get user info
        var loginRequest = new { Username = username, Password = "TestPass123!" };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var authResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

        // Purchase a ship
        var purchaseRequest = new ShipPurchaseDto
        {
            UserId = authResult!.UserId,
            ShipType = "Escape Pod",
            Name = "Test Ship"
        };

        var response = await _client.PostAsJsonAsync("/api/ship/purchase", purchaseRequest);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<ShipPurchaseResultDto>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.ShipId.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Upgrade_Ship_Should_Modify_Ship_Properties()
    {
        // Create a test user and ship first
        var username = $"shiptest{Random.Shared.Next(1000, 9999)}";
        var registerRequest = new { Username = username, Email = $"{username}@testing.com", Password = "TestPass123!" };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginRequest = new { Username = username, Password = "TestPass123!" };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var authResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

        var purchaseRequest = new ShipPurchaseDto
        {
            UserId = authResult!.UserId,
            ShipType = "Escape Pod",
            Name = "Upgrade Test Ship"
        };

        var purchaseResponse = await _client.PostAsJsonAsync("/api/ship/purchase", purchaseRequest);
        purchaseResponse.EnsureSuccessStatusCode();
        var purchaseResult = await purchaseResponse.Content.ReadFromJsonAsync<ShipPurchaseResultDto>();

        // Upgrade the ship
        var upgradeRequest = new ShipUpgradeDto
        {
            ShipId = purchaseResult!.ShipId,
            UpgradeType = "Cargo",
            Level = 2
        };

        var upgradeResponse = await _client.PostAsJsonAsync("/api/ship/upgrade", upgradeRequest);
        upgradeResponse.EnsureSuccessStatusCode();

        // Verify the upgrade
        var shipResponse = await _client.GetAsync($"/api/ship/{purchaseResult.ShipId}");
        shipResponse.EnsureSuccessStatusCode();
        var ship = await shipResponse.Content.ReadFromJsonAsync<ShipDto>();
        
        ship.Should().NotBeNull();
        ship!.CargoCapacity.Should().BeGreaterThan(300); // Should be upgraded from base 300
    }

    [Fact]
    public async Task Get_Combat_Factions_Should_Return_Faction_List()
    {
        var response = await _client.GetAsync("/api/combat/factions");
        response.EnsureSuccessStatusCode();
        
        var factions = await response.Content.ReadFromJsonAsync<List<CombatFactionDto>>();
        factions.Should().NotBeNull();
        factions!.Count.Should().BeGreaterThanOrEqualTo(0);
        
        if (factions.Count > 0)
        {
            var faction = factions[0];
            faction.Id.Should().BeGreaterThan(0);
            faction.Name.Should().NotBeNullOrEmpty();
            faction.Description.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public async Task Get_Faction_By_Id_Should_Return_Valid_Faction()
    {
        // Get a list of factions first
        var factionsResponse = await _client.GetAsync("/api/combat/factions");
        factionsResponse.EnsureSuccessStatusCode();
        var factions = await factionsResponse.Content.ReadFromJsonAsync<List<CombatFactionDto>>();
        factions.Should().NotBeNull();
        factions!.Count.Should().BeGreaterThan(0);

        var factionId = factions[0].Id;
        var response = await _client.GetAsync($"/api/combat/factions/{factionId}");
        response.EnsureSuccessStatusCode();
        
        var faction = await response.Content.ReadFromJsonAsync<CombatFactionDto>();
        faction.Should().NotBeNull();
        faction!.Id.Should().Be(factionId);
        faction.Name.Should().NotBeNullOrEmpty();
        faction.Description.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Initiate_Combat_Should_Create_Combat_Session()
    {
        // Create two test users
        var user1Name = $"combat1{Random.Shared.Next(1000, 9999)}";
        var user2Name = $"combat2{Random.Shared.Next(1000, 9999)}";
        
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

        // Initiate combat
        var combatRequest = new CombatInitiationDto
        {
            AttackerId = auth1Result!.UserId,
            DefenderId = auth2Result!.UserId,
            CombatType = "Ship"
        };

        var response = await _client.PostAsJsonAsync("/api/combat/initiate", combatRequest);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<CombatResultDto>();
        result.Should().NotBeNull();
        result!.CombatId.Should().BeGreaterThan(0);
        result.Status.Should().BeOneOf("InProgress", "Completed");
    }

    [Fact]
    public async Task Get_Combat_History_Should_Return_Past_Combats()
    {
        // Create a test user
        var username = $"combat{Random.Shared.Next(1000, 9999)}";
        var registerRequest = new { Username = username, Email = $"{username}@testing.com", Password = "TestPass123!" };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginRequest = new { Username = username, Password = "TestPass123!" };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var authResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

        // Get combat history
        var response = await _client.GetAsync($"/api/combat/history/{authResult!.UserId}");
        response.EnsureSuccessStatusCode();
        
        var history = await response.Content.ReadFromJsonAsync<List<CombatHistoryDto>>();
        history.Should().NotBeNull();
        // Note: New users might not have combat history yet, which is valid
    }

    [Fact]
    public async Task Deploy_Troops_Should_Create_Troop_Units()
    {
        // Create a test user
        var username = $"troop{Random.Shared.Next(1000, 9999)}";
        var registerRequest = new { Username = username, Email = $"{username}@testing.com", Password = "TestPass123!" };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginRequest = new { Username = username, Password = "TestPass123!" };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var authResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

        // Deploy troops
        var deployRequest = new TroopDeploymentDto
        {
            UserId = authResult!.UserId,
            Level = 1,
            Quantity = 10,
            LocationType = "Planet",
            LocationId = 1
        };

        var response = await _client.PostAsJsonAsync("/api/combat/deploy-troops", deployRequest);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<TroopDeploymentResultDto>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.DeployedCount.Should().Be(10);
    }

    // DTOs for the tests
    private record AuthResponse(int UserId, string Username, string Email, int? CurrentNodeId, int? RealmId, int CargoHolds, DateTime LastLogin, string Token);
    private record ShipDto(int Id, string Name, string ShipType, int CargoCapacity, int WeaponLevel, int ShieldLevel, int ComputerLevel, int? OwnerId);
    private record ShipPurchaseDto
    {
        public int UserId { get; init; }
        public string ShipType { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
    }
    private record ShipPurchaseResultDto(bool Success, int ShipId, string Message);
    private record ShipUpgradeDto
    {
        public int ShipId { get; init; }
        public string UpgradeType { get; init; } = string.Empty;
        public int Level { get; init; }
    }
    private record CombatFactionDto(int Id, string Name, string Description, int PowerLevel);
    private record CombatInitiationDto
    {
        public int AttackerId { get; init; }
        public int DefenderId { get; init; }
        public string CombatType { get; init; } = string.Empty;
    }
    private record CombatResultDto(int CombatId, string Status, string Winner, string Details);
    private record CombatHistoryDto(int CombatId, int AttackerId, int DefenderId, string Result, DateTime Timestamp);
    private record TroopDeploymentDto
    {
        public int UserId { get; init; }
        public int Level { get; init; }
        public int Quantity { get; init; }
        public string LocationType { get; init; } = string.Empty;
        public int LocationId { get; init; }
    }
    private record TroopDeploymentResultDto(bool Success, int DeployedCount, string Message);
}
