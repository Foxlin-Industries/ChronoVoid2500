using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using ChronoVoid.API.Data;
using ChronoVoid.API.Models;
using ChronoVoid.API.DTOs;

namespace ChronoVoid.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NavigationController : ControllerBase
{
    private readonly ChronoVoidContext _context;
    private readonly Microsoft.AspNetCore.SignalR.IHubContext<ChronoVoid.API.Hubs.GalaxyHub> _galaxyHub;

    public NavigationController(ChronoVoidContext context, Microsoft.AspNetCore.SignalR.IHubContext<ChronoVoid.API.Hubs.GalaxyHub> galaxyHub)
    {
        _context = context;
        _galaxyHub = galaxyHub;
    }

    [HttpGet("node/{nodeId}")]
    public async Task<ActionResult<NodeDetailDto>> GetNodeDetails(int nodeId)
    {
        var node = await _context.NeuralNodes
            .Include(n => n.OutgoingTunnels)
            .ThenInclude(t => t.ToNode)
            .Include(n => n.Realm)
            .FirstOrDefaultAsync(n => n.Id == nodeId);

        if (node == null)
            return NotFound("Neural Node not found");

        var nodeDetail = new NodeDetailDto
        {
            Id = node.Id,
            NodeNumber = node.NodeNumber,
            RealmName = node.Realm.Name,

            HasQuantumStation = node.HasQuantumStation,
            StarName = node.StarName,
            PlanetCount = node.PlanetCount,
            ConnectedNodes = node.OutgoingTunnels.Select(t => new ConnectedNodeDto
            {
                NodeId = t.ToNodeId,
                NodeNumber = t.ToNode.NodeNumber,
                HasQuantumStation = t.ToNode.HasQuantumStation,

                StarName = t.ToNode.StarName,
                PlanetCount = t.ToNode.PlanetCount
            }).ToList()
        };

        return nodeDetail;
    }

    [HttpPost("move")]
    public async Task<ActionResult<NavigationResultDto>> MoveToNode(MoveRequestDto request)
    {
        // Verify user exists (simplified for MVP)
        var user = await _context.Users.FindAsync(request.UserId);
        if (user == null)
        {
            // Create user if doesn't exist (for testing)
            user = new User
            {
                Id = request.UserId,
                Username = $"TestUser{request.UserId}",
                Email = $"user{request.UserId}@test.com",
                PasswordHash = "TestPassword", // Temporary for testing
                CurrentNodeId = request.FromNodeId,
                RealmId = await GetRealmIdFromNode(request.FromNodeId)
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        // Verify the movement is valid (tunnel exists)
        var tunnel = await _context.HyperTunnels
            .FirstOrDefaultAsync(t => t.FromNodeId == request.FromNodeId && t.ToNodeId == request.ToNodeId);

        if (tunnel == null)
            return BadRequest("No HyperTunnel exists between these nodes");

        // Update user location
        user.CurrentNodeId = request.ToNodeId;
        user.LastLogin = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Get destination node details
        var destinationNode = await GetNodeDetails(request.ToNodeId);

        // Broadcast movement (basic broadcast for now)
        await _galaxyHub.Clients.All.SendAsync("UserMoved", new {
            request.UserId,
            FromNodeId = request.FromNodeId,
            ToNodeId = request.ToNodeId,
            Timestamp = DateTime.UtcNow
        });

        return Ok(new NavigationResultDto
        {
            Success = true,
            Message = $"Successfully moved to Neural Node {destinationNode.Value?.NodeNumber}",
            CurrentNode = destinationNode.Value!,
            Timestamp = DateTime.UtcNow
        });
    }

    [HttpGet("user/{userId}/location")]
    public async Task<ActionResult<UserLocationDto>> GetUserLocation(int userId)
    {
        var user = await _context.Users
            .Include(u => u.CurrentNode)
            .Include(u => u.Realm)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return NotFound("User not found");

        if (user.CurrentNode == null)
            return NotFound("User location not set");

        return Ok(new UserLocationDto
        {
            UserId = user.Id,
            Username = user.Username,
            CurrentNodeId = user.CurrentNode.Id,
            CurrentNodeNumber = user.CurrentNode.NodeNumber,
            RealmId = user.RealmId ?? 0,
            RealmName = user.Realm?.Name ?? "Unknown",
            LastLogin = user.LastLogin
        });
    }

    [HttpGet("realm/{realmId}/users")]
    public async Task<ActionResult<IEnumerable<UserLocationDto>>> GetUsersInRealm(int realmId)
    {
        var users = await _context.Users
            .Include(u => u.CurrentNode)
            .Include(u => u.Realm)
            .Where(u => u.RealmId == realmId && u.CurrentNode != null)
            .Select(u => new UserLocationDto
            {
                UserId = u.Id,
                Username = u.Username,
                CurrentNodeId = u.CurrentNode!.Id,
                CurrentNodeNumber = u.CurrentNode.NodeNumber,
                RealmId = u.RealmId ?? 0,
                RealmName = u.Realm!.Name,
                LastLogin = u.LastLogin
            })
            .ToListAsync();

        return Ok(users);
    }

    private async Task<int?> GetRealmIdFromNode(int nodeId)
    {
        var node = await _context.NeuralNodes.FindAsync(nodeId);
        return node?.RealmId;
    }
}