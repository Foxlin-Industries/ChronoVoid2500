using System.Net;
using System.Net.Http.Json;
using ChronoVoid2500.UnitTests.TestInfrastructure;
using FluentAssertions;

namespace ChronoVoid2500.UnitTests.Api;

public class TradingSystemTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public TradingSystemTests(ApiTestFixture fx)
    {
        _client = fx.Client;
    }

    [Fact]
    public async Task Complete_Trading_Cycle_With_Player3333()
    {
        // Step 1: Create player3333 with specified credentials
        var playerId = 3333;
        var username = $"player{playerId}";
        var email = $"{username}@testing.com";
        var password = "SecureTest123!";

        // Register the player
        var registerRequest = new { Username = username, Email = email, Password = password };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Login to get authentication token
        var loginRequest = new { Username = username, Password = password };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var authResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        authResult.Should().NotBeNull();
        authResult!.UserId.Should().Be(playerId);

        // Add auth token to subsequent requests
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResult.Token);

        // Step 2: Get available realms and select the first test realm
        var realmsResponse = await _client.GetAsync("/api/realm");
        realmsResponse.EnsureSuccessStatusCode();
        var realms = await realmsResponse.Content.ReadFromJsonAsync<List<RealmDto>>();
        realms.Should().NotBeNull();
        realms!.Count.Should().BeGreaterThan(0);
        var testRealm = realms[0];

        // Step 3: Get nodes in the test realm
        var nodesResponse = await _client.GetAsync($"/api/realm/{testRealm.Id}/nodes");
        nodesResponse.EnsureSuccessStatusCode();
        var nodes = await nodesResponse.Content.ReadFromJsonAsync<List<NodeDto>>();
        nodes.Should().NotBeNull();
        nodes!.Count.Should().BeGreaterThan(0);

        // Find sector 1 (first node with starbase)
        var sector1Node = nodes.FirstOrDefault(n => n.HasQuantumStation);
        sector1Node.Should().NotBeNull("Sector 1 (node with starbase) should exist");

        // Step 4: Navigate to sector 1 and dock at starbase
        var moveToSector1Request = new { UserId = playerId, FromNodeId = authResult.CurrentNodeId ?? 1, ToNodeId = sector1Node!.Id };
        var moveToSector1Response = await _client.PostAsJsonAsync("/api/navigation/move", moveToSector1Request);
        moveToSector1Response.EnsureSuccessStatusCode();

        // Step 5: Get starbase information for sector 1
        var starbaseResponse = await _client.GetAsync($"/api/trade/starbase/by-node/{sector1Node.Id}");
        starbaseResponse.EnsureSuccessStatusCode();
        var starbaseInfo = await starbaseResponse.Content.ReadFromJsonAsync<StarbaseInfo>();
        starbaseInfo.Should().NotBeNull();

        // Step 6: Get market at starbase and buy maximum of lowest cost resource
        var marketResponse = await _client.GetAsync($"/api/trade/starbase/{starbaseInfo!.Id}/market");
        marketResponse.EnsureSuccessStatusCode();
        var market = await marketResponse.Content.ReadFromJsonAsync<MarketDto>();
        market.Should().NotBeNull();
        market!.Items.Should().NotBeEmpty();

        // Find the lowest cost resource
        var lowestCostItem = market.Items.OrderBy(i => i.BuyPrice).First();
        var maxQuantity = Math.Min(lowestCostItem.Stock, 100); // Assume player can afford 100 units

        // Record initial credits (we'll need to implement this)
        var initialCredits = await GetPlayerCredits(playerId);

        // Buy maximum of lowest cost resource
        var buyRequest = new TradeRequestDto
        {
            UserId = playerId,
            StarbaseId = starbaseInfo.Id,
            ResourceType = lowestCostItem.ResourceType,
            Quantity = maxQuantity,
            IsBuy = true
        };
        var buyResponse = await _client.PostAsJsonAsync("/api/trade/trade", buyRequest);
        buyResponse.EnsureSuccessStatusCode();

        // Step 7: Navigate to sector 2 (find next node with starbase)
        var sector2Node = nodes.FirstOrDefault(n => n.Id != sector1Node.Id && n.HasQuantumStation);
        if (sector2Node != null)
        {
            var moveToSector2Request = new { UserId = playerId, FromNodeId = sector1Node.Id, ToNodeId = sector2Node.Id };
            var moveToSector2Response = await _client.PostAsJsonAsync("/api/navigation/move", moveToSector2Request);
            moveToSector2Response.EnsureSuccessStatusCode();

            // Get starbase in sector 2
            var starbase2Response = await _client.GetAsync($"/api/trade/starbase/by-node/{sector2Node.Id}");
            if (starbase2Response.IsSuccessStatusCode)
            {
                var starbase2Info = await starbase2Response.Content.ReadFromJsonAsync<StarbaseInfo>();
                
                // Sell the resources bought in sector 1
                var sellRequest = new TradeRequestDto
                {
                    UserId = playerId,
                    StarbaseId = starbase2Info!.Id,
                    ResourceType = lowestCostItem.ResourceType,
                    Quantity = maxQuantity,
                    IsBuy = false
                };
                var sellResponse = await _client.PostAsJsonAsync("/api/trade/trade", sellRequest);
                sellResponse.EnsureSuccessStatusCode();

                // Buy maximum of next level resource
                var market2Response = await _client.GetAsync($"/api/trade/starbase/{starbase2Info.Id}/market");
                market2Response.EnsureSuccessStatusCode();
                var market2 = await market2Response.Content.ReadFromJsonAsync<MarketDto>();
                var nextLevelItem = market2!.Items.OrderBy(i => i.BuyPrice).Skip(1).FirstOrDefault() ?? market2.Items.First();
                var maxQuantity2 = Math.Min(nextLevelItem.Stock, 50);

                var buy2Request = new TradeRequestDto
                {
                    UserId = playerId,
                    StarbaseId = starbase2Info.Id,
                    ResourceType = nextLevelItem.ResourceType,
                    Quantity = maxQuantity2,
                    IsBuy = true
                };
                var buy2Response = await _client.PostAsJsonAsync("/api/trade/trade", buy2Request);
                buy2Response.EnsureSuccessStatusCode();

                // Step 8: Navigate to sector 3 (or back to sector 1 if no sector 3)
                var sector3Node = nodes.FirstOrDefault(n => n.Id != sector1Node.Id && n.Id != sector2Node.Id && n.HasQuantumStation);
                var targetSector3 = sector3Node ?? sector1Node; // Fall back to sector 1 if no sector 3

                var moveToSector3Request = new { UserId = playerId, FromNodeId = sector2Node.Id, ToNodeId = targetSector3.Id };
                var moveToSector3Response = await _client.PostAsJsonAsync("/api/navigation/move", moveToSector3Request);
                moveToSector3Response.EnsureSuccessStatusCode();

                // Sell the resources from sector 2
                var starbase3Response = await _client.GetAsync($"/api/trade/starbase/by-node/{targetSector3.Id}");
                if (starbase3Response.IsSuccessStatusCode)
                {
                    var starbase3Info = await starbase3Response.Content.ReadFromJsonAsync<StarbaseInfo>();
                    
                    var sell2Request = new TradeRequestDto
                    {
                        UserId = playerId,
                        StarbaseId = starbase3Info!.Id,
                        ResourceType = nextLevelItem.ResourceType,
                        Quantity = maxQuantity2,
                        IsBuy = false
                    };
                    var sell2Response = await _client.PostAsJsonAsync("/api/trade/trade", sell2Request);
                    sell2Response.EnsureSuccessStatusCode();

                    // Buy maximum of lowest cost resource again
                    var market3Response = await _client.GetAsync($"/api/trade/starbase/{starbase3Info.Id}/market");
                    market3Response.EnsureSuccessStatusCode();
                    var market3 = await market3Response.Content.ReadFromJsonAsync<MarketDto>();
                    var lowestCostItem3 = market3!.Items.OrderBy(i => i.BuyPrice).First();
                    var maxQuantity3 = Math.Min(lowestCostItem3.Stock, 75);

                    var buy3Request = new TradeRequestDto
                    {
                        UserId = playerId,
                        StarbaseId = starbase3Info.Id,
                        ResourceType = lowestCostItem3.ResourceType,
                        Quantity = maxQuantity3,
                        IsBuy = true
                    };
                    var buy3Response = await _client.PostAsJsonAsync("/api/trade/trade", buy3Request);
                    buy3Response.EnsureSuccessStatusCode();

                    // Step 9: Navigate back to sector 1 and sell final resources
                    var moveBackToSector1Request = new { UserId = playerId, FromNodeId = targetSector3.Id, ToNodeId = sector1Node.Id };
                    var moveBackResponse = await _client.PostAsJsonAsync("/api/navigation/move", moveBackToSector1Request);
                    moveBackResponse.EnsureSuccessStatusCode();

                    var finalSellRequest = new TradeRequestDto
                    {
                        UserId = playerId,
                        StarbaseId = starbaseInfo.Id,
                        ResourceType = lowestCostItem3.ResourceType,
                        Quantity = maxQuantity3,
                        IsBuy = false
                    };
                    var finalSellResponse = await _client.PostAsJsonAsync("/api/trade/trade", finalSellRequest);
                    finalSellResponse.EnsureSuccessStatusCode();

                    // Step 10: Verify success criteria
                    var finalCredits = await GetPlayerCredits(playerId);
                    
                    // Success criteria: No errors occurred and credits changed
                    finalCredits.Should().NotBe(initialCredits, "Credits should have changed after trading cycle");
                    
                    // Verify no errors occurred (all responses were successful)
                    registerResponse.IsSuccessStatusCode.Should().BeTrue();
                    loginResponse.IsSuccessStatusCode.Should().BeTrue();
                    moveToSector1Response.IsSuccessStatusCode.Should().BeTrue();
                    starbaseResponse.IsSuccessStatusCode.Should().BeTrue();
                    marketResponse.IsSuccessStatusCode.Should().BeTrue();
                    buyResponse.IsSuccessStatusCode.Should().BeTrue();
                    moveToSector2Response.IsSuccessStatusCode.Should().BeTrue();
                    sellResponse.IsSuccessStatusCode.Should().BeTrue();
                    buy2Response.IsSuccessStatusCode.Should().BeTrue();
                    moveToSector3Response.IsSuccessStatusCode.Should().BeTrue();
                    sell2Response.IsSuccessStatusCode.Should().BeTrue();
                    buy3Response.IsSuccessStatusCode.Should().BeTrue();
                    moveBackResponse.IsSuccessStatusCode.Should().BeTrue();
                    finalSellResponse.IsSuccessStatusCode.Should().BeTrue();
                }
            }
        }
    }

    private async Task<int> GetPlayerCredits(int userId)
    {
        // This would need to be implemented in the API
        // For now, return a placeholder value
        return 1000;
    }

    // DTOs for the test
    private record AuthResponse(int UserId, string Username, string Email, int? CurrentNodeId, int? RealmId, int CargoHolds, DateTime LastLogin, string Token);
    private record RealmDto(int Id, string Name, int NodeCount, int QuantumStationSeedRate, bool NoDeadNodes, DateTime CreatedAt, bool IsActive);
    private record NodeDto(int Id, int NodeNumber, int CoordinateX, int CoordinateY, bool HasQuantumStation, List<int> ConnectedNodes);
    private record StarbaseInfo(int Id, int NodeId, int? OwnerId, int DefenseLevel);
    private record MarketDto(int StarbaseId, string StarName, List<MarketItemDto> Items);
    private record MarketItemDto(int StarbaseId, string ResourceType, decimal BuyPrice, decimal SellPrice, int Stock);
    private record TradeRequestDto(int UserId, int StarbaseId, string ResourceType, int Quantity, bool IsBuy);
}
