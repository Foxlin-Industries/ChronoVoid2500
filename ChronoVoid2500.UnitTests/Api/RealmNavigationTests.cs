using System.Net.Http.Json;
using ChronoVoid2500.UnitTests.TestInfrastructure;
using FluentAssertions;

namespace ChronoVoid2500.UnitTests.Api;

public class RealmNavigationTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public RealmNavigationTests(ApiTestFixture fx)
    {
        _client = fx.Client;
    }

    [Fact]
    public async Task GetRealms_And_NodeDetails()
    {
        var realms = await _client.GetFromJsonAsync<List<RealmDto>>("/api/realm");
        realms.Should().NotBeNull();
        realms!.Count.Should().BeGreaterThan(0);
        var realm = realms[0];

        var nodes = await _client.GetFromJsonAsync<List<NodeDto>>($"/api/realm/{realm.Id}/nodes");
        nodes.Should().NotBeNull();
        nodes!.Count.Should().BeGreaterThan(0);

        var nodeId = nodes[0].Id;
        var nodeDetail = await _client.GetFromJsonAsync<NodeDetailDto>($"/api/navigation/node/{nodeId}");
        nodeDetail.Should().NotBeNull();
        nodeDetail!.Id.Should().Be(nodeId);
    }

    private record RealmDto(int Id, string Name, int NodeCount, int QuantumStationSeedRate, bool NoDeadNodes, DateTime CreatedAt, bool IsActive);
    private record NodeDto(int Id, int NodeNumber, int CoordinateX, int CoordinateY, bool HasQuantumStation, List<int> ConnectedNodes);
    private record NodeDetailDto(int Id, int NodeNumber, string RealmName, bool HasQuantumStation, string StarName, int PlanetCount, List<Conn> ConnectedNodes);
    private record Conn(int NodeId, int NodeNumber, bool HasQuantumStation, string StarName, int PlanetCount);
}

