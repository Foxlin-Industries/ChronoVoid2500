using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChronoVoid.API.Data;
using ChronoVoid.API.Models;
using ChronoVoid.API.Services;
using ChronoVoid.API.DTOs;

namespace ChronoVoid.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RealmController : ControllerBase
{
    private readonly ChronoVoidContext _context;
    private readonly RealmGenerationService _realmService;

    public RealmController(ChronoVoidContext context, RealmGenerationService realmService)
    {
        _context = context;
        _realmService = realmService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RealmDto>>> GetRealms()
    {
        var realms = await _context.NexusRealms
            .Where(r => r.IsActive)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new RealmDto
            {
                Id = r.Id,
                Name = r.Name,
                NodeCount = r.NodeCount,
                QuantumStationSeedRate = r.QuantumStationSeedRate,
                NoDeadNodes = r.NoDeadNodes,
                CreatedAt = r.CreatedAt,
                IsActive = r.IsActive
            })
            .ToListAsync();
        
        return realms;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RealmDto>> GetRealm(int id)
    {
        var realm = await _context.NexusRealms
            .Where(r => r.Id == id)
            .Select(r => new RealmDto
            {
                Id = r.Id,
                Name = r.Name,
                NodeCount = r.NodeCount,
                QuantumStationSeedRate = r.QuantumStationSeedRate,
                NoDeadNodes = r.NoDeadNodes,
                CreatedAt = r.CreatedAt,
                IsActive = r.IsActive
            })
            .FirstOrDefaultAsync();

        if (realm == null)
            return NotFound();

        return realm;
    }

    [HttpPost]
    public async Task<ActionResult<RealmDto>> CreateRealm(CreateRealmRequest request)
    {
        try
        {
            var realm = await _realmService.CreateRealmAsync(
                request.Name, 
                request.NodeCount, 
                request.QuantumStationSeedRate, 
                request.NoDeadNodes);

            var realmDto = new RealmDto
            {
                Id = realm.Id,
                Name = realm.Name,
                NodeCount = realm.NodeCount,
                QuantumStationSeedRate = realm.QuantumStationSeedRate,
                NoDeadNodes = realm.NoDeadNodes,
                CreatedAt = realm.CreatedAt,
                IsActive = realm.IsActive
            };
            
            return CreatedAtAction(nameof(GetRealm), new { id = realm.Id }, realmDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error creating realm: {ex.Message}");
        }
    }

    [HttpGet("{id}/nodes")]
    public async Task<ActionResult<IEnumerable<object>>> GetRealmNodes(int id)
    {
        var nodes = await _context.NeuralNodes
            .Where(n => n.RealmId == id)
            .Include(n => n.OutgoingTunnels)
            .Select(n => new
            {
                n.Id,
                n.NodeNumber,
                n.CoordinateX,
                n.CoordinateY,
                n.HasQuantumStation,
                ConnectedNodes = n.OutgoingTunnels.Select(t => t.ToNodeId).ToList()
            })
            .OrderBy(n => n.NodeNumber)
            .ToListAsync();

        if (!nodes.Any())
            return NotFound("Realm not found or has no nodes");

        return Ok(nodes);
    }

    [HttpGet("{id}/connectivity-test")]
    public async Task<ActionResult<object>> TestConnectivity(int id)
    {
        var realm = await _context.NexusRealms.FindAsync(id);
        if (realm == null) return NotFound();

        var nodes = await _context.NeuralNodes
            .Where(n => n.RealmId == id)
            .Include(n => n.OutgoingTunnels)
            .ToListAsync();

        var deadNodes = nodes.Where(n => !n.OutgoingTunnels.Any()).ToList();
        var node1Connections = nodes.First(n => n.NodeNumber == 1).OutgoingTunnels.Count;

        return Ok(new
        {
            RealmName = realm.Name,
            TotalNodes = nodes.Count,
            DeadNodes = deadNodes.Count,
            DeadNodeNumbers = deadNodes.Select(n => n.NodeNumber).ToList(),
            Node1Connections = node1Connections,
            HasNoDeadNodes = deadNodes.Count == 0,
            QuantumStations = nodes.Count(n => n.HasQuantumStation)
        });
    }
}

