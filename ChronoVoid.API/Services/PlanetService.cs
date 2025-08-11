using ChronoVoid.API.Data;
using ChronoVoid.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ChronoVoid.API.Services;

public class PlanetService
{
    private readonly ChronoVoidContext _context;
    
    // Solar system planets in order
    private static readonly string[] PlanetNames = 
    {
        "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune", "Pluto"
    };
    
    // Planet sizes: 1=Tiny, 2,3,4=Average, 5,6,7,8=Huge, 9=Tiny
    private static readonly PlanetSize[] PlanetSizes = 
    {
        PlanetSize.Tiny,    // Mercury
        PlanetSize.Average, // Venus
        PlanetSize.Average, // Earth
        PlanetSize.Average, // Mars
        PlanetSize.Huge,    // Jupiter
        PlanetSize.Huge,    // Saturn
        PlanetSize.Huge,    // Uranus
        PlanetSize.Huge,    // Neptune
        PlanetSize.Tiny     // Pluto
    };

    public PlanetService(ChronoVoidContext context)
    {
        _context = context;
    }

    public async Task SeedPlanetsForAllRealmsAsync()
    {
        var realms = await _context.NexusRealms
            .Include(r => r.Nodes)
            .ThenInclude(n => n.Planets)
            .ToListAsync();

        foreach (var realm in realms)
        {
            await SeedPlanetsForRealmAsync(realm);
        }

        await _context.SaveChangesAsync();
    }

    public async Task SeedPlanetsForRealmAsync(NexusRealm realm)
    {
        // Load nodes with planets if not already loaded
        if (!realm.Nodes.Any() || !realm.Nodes.First().Planets.Any())
        {
            await _context.Entry(realm)
                .Collection(r => r.Nodes)
                .Query()
                .Include(n => n.Planets)
                .Include(n => n.Starbase)
                .LoadAsync();
        }

        foreach (var node in realm.Nodes)
        {
            // Add planets if they don't exist
            if (!node.Planets.Any())
            {
                for (int i = 0; i < 9; i++)
                {
                    var planet = new Planet
                    {
                        NodeId = node.Id,
                        PlanetNumber = i + 1,
                        Name = PlanetNames[i],
                        Size = PlanetSizes[i],
                        CreatedAt = DateTime.UtcNow
                    };
                    
                    _context.Planets.Add(planet);
                }
                
                // Update the node's planet count
                node.PlanetCount = 9;
            }

            // Add starbase to Node 1 if it doesn't exist
            if (node.NodeNumber == 1 && node.Starbase == null)
            {
                var starbase = new Starbase
                {
                    NodeId = node.Id,
                    DefenseLevel = 1,
                    CreatedAt = DateTime.UtcNow
                };
                
                _context.Starbases.Add(starbase);
                node.HasQuantumStation = true;
            }
        }
    }

    public async Task<Planet> AddPlanetToNodeAsync(int nodeId, string planetName, PlanetSize size)
    {
        var node = await _context.NeuralNodes
            .Include(n => n.Planets)
            .FirstOrDefaultAsync(n => n.Id == nodeId);

        if (node == null)
            throw new ArgumentException("Node not found");

        var nextPlanetNumber = node.Planets.Any() ? node.Planets.Max(p => p.PlanetNumber) + 1 : 1;

        var planet = new Planet
        {
            NodeId = nodeId,
            PlanetNumber = nextPlanetNumber,
            Name = planetName,
            Size = size,
            CreatedAt = DateTime.UtcNow
        };

        _context.Planets.Add(planet);
        
        // Update planet count
        node.PlanetCount = node.Planets.Count + 1;
        
        await _context.SaveChangesAsync();
        return planet;
    }

    public async Task<bool> RemovePlanetAsync(int planetId)
    {
        var planet = await _context.Planets
            .Include(p => p.Node)
            .FirstOrDefaultAsync(p => p.Id == planetId);

        if (planet == null)
            return false;

        _context.Planets.Remove(planet);
        
        // Update planet count
        planet.Node.PlanetCount = Math.Max(0, planet.Node.PlanetCount - 1);
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Starbase> AddStarbaseToNodeAsync(int nodeId)
    {
        var node = await _context.NeuralNodes
            .Include(n => n.Starbase)
            .FirstOrDefaultAsync(n => n.Id == nodeId);

        if (node == null)
            throw new ArgumentException("Node not found");

        if (node.Starbase != null)
            throw new InvalidOperationException("Node already has a starbase");

        var starbase = new Starbase
        {
            NodeId = nodeId,
            DefenseLevel = 1,
            CreatedAt = DateTime.UtcNow
        };

        _context.Starbases.Add(starbase);
        node.HasQuantumStation = true;
        
        await _context.SaveChangesAsync();
        return starbase;
    }

    public async Task<bool> RemoveStarbaseAsync(int nodeId)
    {
        var starbase = await _context.Starbases
            .Include(s => s.Node)
            .FirstOrDefaultAsync(s => s.NodeId == nodeId);

        if (starbase == null)
            return false;

        _context.Starbases.Remove(starbase);
        starbase.Node.HasQuantumStation = false;
        
        await _context.SaveChangesAsync();
        return true;
    }
}