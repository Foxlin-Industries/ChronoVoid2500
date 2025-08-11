using ChronoVoid.API.Data;
using ChronoVoid.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ChronoVoid.API.Services;

public class RealmGenerationService
{
    private readonly ChronoVoidContext _context;
    private readonly Random _random;
    private readonly PlanetService _planetService;

    public RealmGenerationService(ChronoVoidContext context, PlanetService planetService)
    {
        _context = context;
        _planetService = planetService;
        _random = new Random();
    }

    public async Task<NexusRealm> CreateRealmAsync(string name, int nodeCount, int quantumStationSeedRate, bool noDeadNodes)
    {
        // Validate parameters
        if (nodeCount < 10) throw new ArgumentException("Minimum 10 nodes required");
        if (quantumStationSeedRate < 0 || quantumStationSeedRate > 100) 
            throw new ArgumentException("Quantum Station seed rate must be 0-100%");

        // Create the realm
        var realm = new NexusRealm
        {
            Name = name,
            NodeCount = nodeCount,
            QuantumStationSeedRate = quantumStationSeedRate,
            NoDeadNodes = noDeadNodes
        };

        _context.NexusRealms.Add(realm);
        await _context.SaveChangesAsync();

        // Generate nodes
        await GenerateNodesAsync(realm);
        
        // Generate HyperTunnels with your special rules
        await GenerateHyperTunnelsAsync(realm, noDeadNodes);
        
        // Seed Quantum Stations
        await SeedQuantumStationsAsync(realm, quantumStationSeedRate);
        
        // Seed Planets and Starbases
        await _planetService.SeedPlanetsForRealmAsync(realm);

        return realm;
    }

    private async Task GenerateNodesAsync(NexusRealm realm)
    {
        var nodes = new List<NeuralNode>();
        
        // Generate nodes in a rough grid pattern for better connectivity
        int gridSize = (int)Math.Ceiling(Math.Sqrt(realm.NodeCount));
        
        for (int i = 1; i <= realm.NodeCount; i++)
        {
            // Calculate grid position with some randomization
            int baseX = ((i - 1) % gridSize) * 100;
            int baseY = ((i - 1) / gridSize) * 100;
            
            var node = new NeuralNode
            {
                RealmId = realm.Id,
                NodeNumber = i,
                CoordinateX = baseX + _random.Next(-25, 26), // Add some randomness
                CoordinateY = baseY + _random.Next(-25, 26),
                HasQuantumStation = false, // Will be set later
                StarName = GenerateStarName(),
                PlanetCount = 9 // All nodes will have 9 planets (solar system)
            };
            
            nodes.Add(node);
        }

        _context.NeuralNodes.AddRange(nodes);
        await _context.SaveChangesAsync();
    }

    private async Task GenerateHyperTunnelsAsync(NexusRealm realm, bool noDeadNodes)
    {
        var nodes = await _context.NeuralNodes
            .Where(n => n.RealmId == realm.Id)
            .OrderBy(n => n.NodeNumber)
            .ToListAsync();

        var tunnels = new List<HyperTunnel>();

        // Node 1 special connections (your requirement)
        var node1 = nodes.First();
        var lastNode = nodes.Last();
        
        // Connect Node 1 to nodes 2-9
        for (int i = 2; i <= Math.Min(9, realm.NodeCount); i++)
        {
            var targetNode = nodes.FirstOrDefault(n => n.NodeNumber == i);
            if (targetNode != null)
            {
                // Bidirectional tunnels
                tunnels.Add(new HyperTunnel { FromNodeId = node1.Id, ToNodeId = targetNode.Id });
                tunnels.Add(new HyperTunnel { FromNodeId = targetNode.Id, ToNodeId = node1.Id });
            }
        }

        // Connect Node 1 to last node
        if (lastNode.NodeNumber > 9)
        {
            tunnels.Add(new HyperTunnel { FromNodeId = node1.Id, ToNodeId = lastNode.Id });
            tunnels.Add(new HyperTunnel { FromNodeId = lastNode.Id, ToNodeId = node1.Id });
        }

        // Connect Node 1 to one random node
        var randomNode = nodes.Where(n => n.NodeNumber > 9 && n.NodeNumber < realm.NodeCount)
                             .OrderBy(x => _random.Next())
                             .FirstOrDefault();
        if (randomNode != null)
        {
            tunnels.Add(new HyperTunnel { FromNodeId = node1.Id, ToNodeId = randomNode.Id });
            tunnels.Add(new HyperTunnel { FromNodeId = randomNode.Id, ToNodeId = node1.Id });
        }

        // Generate additional connections for other nodes
        await GenerateAdditionalConnections(nodes, tunnels, noDeadNodes);

        _context.HyperTunnels.AddRange(tunnels);
        await _context.SaveChangesAsync();
    }

    private async Task GenerateAdditionalConnections(List<NeuralNode> nodes, List<HyperTunnel> tunnels, bool noDeadNodes)
    {
        var existingConnections = new HashSet<(int, int)>();
        
        // Track existing connections
        foreach (var tunnel in tunnels)
        {
            existingConnections.Add((tunnel.FromNodeId, tunnel.ToNodeId));
        }

        // Connect each node to 2-4 nearby nodes (excluding Node 1 which already has its connections)
        foreach (var node in nodes.Where(n => n.NodeNumber > 1))
        {
            var connectionsCount = GetConnectionCount(node.Id, tunnels);
            var targetConnections = _random.Next(2, 5); // 2-4 connections per node

            if (connectionsCount >= targetConnections) continue;

            // Find nearby nodes to connect to
            var nearbyNodes = nodes
                .Where(n => n.Id != node.Id)
                .OrderBy(n => Math.Sqrt(Math.Pow(n.CoordinateX - node.CoordinateX, 2) + 
                                      Math.Pow(n.CoordinateY - node.CoordinateY, 2)))
                .Take(10) // Consider 10 nearest nodes
                .ToList();

            foreach (var nearbyNode in nearbyNodes)
            {
                if (connectionsCount >= targetConnections) break;

                // Check if connection already exists
                if (existingConnections.Contains((node.Id, nearbyNode.Id)) ||
                    existingConnections.Contains((nearbyNode.Id, node.Id)))
                    continue;

                // Add bidirectional connection
                tunnels.Add(new HyperTunnel { FromNodeId = node.Id, ToNodeId = nearbyNode.Id });
                tunnels.Add(new HyperTunnel { FromNodeId = nearbyNode.Id, ToNodeId = node.Id });
                
                existingConnections.Add((node.Id, nearbyNode.Id));
                existingConnections.Add((nearbyNode.Id, node.Id));
                
                connectionsCount += 2; // Bidirectional
            }
        }

        // If NoDeadNodes is enabled, ensure every node has at least one connection
        if (noDeadNodes)
        {
            await EnsureNoDeadNodes(nodes, tunnels, existingConnections);
        }
    }

    private Task EnsureNoDeadNodes(List<NeuralNode> nodes, List<HyperTunnel> tunnels, 
                                        HashSet<(int, int)> existingConnections)
    {
        foreach (var node in nodes)
        {
            if (GetConnectionCount(node.Id, tunnels) == 0)
            {
                // Find the nearest connected node
                var nearestNode = nodes
                    .Where(n => n.Id != node.Id && GetConnectionCount(n.Id, tunnels) > 0)
                    .OrderBy(n => Math.Sqrt(Math.Pow(n.CoordinateX - node.CoordinateX, 2) + 
                                          Math.Pow(n.CoordinateY - node.CoordinateY, 2)))
                    .First();

                // Connect to nearest node (bidirectional)
                tunnels.Add(new HyperTunnel { FromNodeId = node.Id, ToNodeId = nearestNode.Id });
                tunnels.Add(new HyperTunnel { FromNodeId = nearestNode.Id, ToNodeId = node.Id });
                
                existingConnections.Add((node.Id, nearestNode.Id));
                existingConnections.Add((nearestNode.Id, node.Id));
            }
        }
        
        return Task.CompletedTask;
    }

    private static int GetConnectionCount(int nodeId, List<HyperTunnel> tunnels)
    {
        return tunnels.Count(t => t.FromNodeId == nodeId || t.ToNodeId == nodeId);
    }

    private async Task SeedQuantumStationsAsync(NexusRealm realm, int seedRate)
    {
        var nodes = await _context.NeuralNodes
            .Where(n => n.RealmId == realm.Id)
            .ToListAsync();

        foreach (var node in nodes)
        {
            if (_random.Next(1, 101) <= seedRate) // Percentage chance
            {
                node.HasQuantumStation = true;
            }
        }

        await _context.SaveChangesAsync();
    }

    private string GenerateStarName()
    {
        var prefixes = new[] { "Alpha", "Beta", "Gamma", "Delta", "Epsilon", "Zeta", "Eta", "Theta", "Iota", "Kappa", "Lambda", "Mu", "Nu", "Xi", "Omicron", "Pi", "Rho", "Sigma", "Tau", "Upsilon", "Phi", "Chi", "Psi", "Omega" };
        var suffixes = new[] { "Centauri", "Draconis", "Orionis", "Cygni", "Lyrae", "Aquilae", "Boötis", "Virginis", "Leonis", "Ursae", "Andromedae", "Cassiopeiae", "Persei", "Aurigae", "Geminorum", "Tauri", "Arietis", "Piscium", "Aquarii", "Capricorni", "Sagittarii", "Scorpii", "Librae", "Cancri" };
        
        var prefix = prefixes[_random.Next(prefixes.Length)];
        var suffix = suffixes[_random.Next(suffixes.Length)];
        
        return $"{prefix} {suffix}";
    }
}