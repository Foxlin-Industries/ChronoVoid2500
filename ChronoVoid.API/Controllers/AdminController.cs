using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChronoVoid.API.Data;
using ChronoVoid.API.Models;
using ChronoVoid.API.DTOs;
using ChronoVoid.API.Services;

namespace ChronoVoid.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly ChronoVoidContext _context;
    private readonly PlanetService _planetService;

    public AdminController(ChronoVoidContext context, PlanetService planetService)
    {
        _context = context;
        _planetService = planetService;
    }

    [HttpGet("users")]
    public async Task<ActionResult<List<UserAdminDto>>> GetAllUsers()
    {
        var users = await _context.Users
            .Include(u => u.Realm)
            .Include(u => u.CurrentNode)
            .Select(u => new UserAdminDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                CargoHolds = u.CargoHolds,
                CreatedAt = u.CreatedAt,
                LastLogin = u.LastLogin,
                RealmId = u.RealmId,
                RealmName = u.Realm != null ? u.Realm.Name : null,
                CurrentNodeId = u.CurrentNodeId,
                CurrentNodeNumber = u.CurrentNode != null ? u.CurrentNode.NodeNumber : null,
                IsActive = u.LastLogin > DateTime.UtcNow.AddDays(-7)
            })
            .OrderByDescending(u => u.LastLogin)
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("users/{userId}")]
    public async Task<ActionResult<UserDetailDto>> GetUserDetail(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Realm)
            .Include(u => u.CurrentNode)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return NotFound("User not found");
        }

        // Get user's realm history (if they've been in multiple realms)
        var realmHistory = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new RealmHistoryDto
            {
                RealmId = u.RealmId,
                RealmName = u.Realm != null ? u.Realm.Name : "No Realm",
                JoinedAt = u.CreatedAt, // For now, using created date
                CurrentNodeId = u.CurrentNodeId,
                CurrentNodeNumber = u.CurrentNode != null ? u.CurrentNode.NodeNumber : null,
                IsCurrentRealm = true
            })
            .ToListAsync();

        var userDetail = new UserDetailDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CargoHolds = user.CargoHolds,
            CreatedAt = user.CreatedAt,
            LastLogin = user.LastLogin,
            CurrentRealmId = user.RealmId,
            CurrentRealmName = user.Realm?.Name,
            CurrentNodeId = user.CurrentNodeId,
            CurrentNodeNumber = user.CurrentNode?.NodeNumber,
            RealmHistory = realmHistory
        };

        return Ok(userDetail);
    }

    [HttpDelete("users/{userId}")]
    public async Task<ActionResult> DeleteUser(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound("User not found");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return Ok(new { Message = $"User {user.Username} deleted successfully" });
    }

    [HttpPost("users/{userId}/reset-location")]
    public async Task<ActionResult> ResetUserLocation(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound("User not found");
        }

        // Reset user to no realm and no current node
        user.RealmId = null;
        user.CurrentNodeId = null;
        
        await _context.SaveChangesAsync();

        return Ok(new { Message = $"User {user.Username} location reset successfully" });
    }

    [HttpGet("realms")]
    public async Task<ActionResult<List<RealmAdminDto>>> GetAllRealms()
    {
        var realms = await _context.NexusRealms
            .Include(r => r.Users)
            .Select(r => new RealmAdminDto
            {
                Id = r.Id,
                Name = r.Name,
                NodeCount = r.NodeCount,
                QuantumStationSeedRate = r.QuantumStationSeedRate,
                NoDeadNodes = r.NoDeadNodes,
                CreatedAt = r.CreatedAt,
                UserCount = r.Users.Count,
                ActiveUserCount = r.Users.Count(u => u.LastLogin > DateTime.UtcNow.AddDays(-7))
            })
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return Ok(realms);
    }

    [HttpGet("realms/{realmId}/users")]
    public async Task<ActionResult<List<UserAdminDto>>> GetRealmUsers(int realmId)
    {
        var realm = await _context.NexusRealms.FindAsync(realmId);
        if (realm == null)
        {
            return NotFound("Realm not found");
        }

        var users = await _context.Users
            .Include(u => u.CurrentNode)
            .Where(u => u.RealmId == realmId)
            .Select(u => new UserAdminDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                CargoHolds = u.CargoHolds,
                CreatedAt = u.CreatedAt,
                LastLogin = u.LastLogin,
                RealmId = u.RealmId,
                RealmName = realm.Name,
                CurrentNodeId = u.CurrentNodeId,
                CurrentNodeNumber = u.CurrentNode != null ? u.CurrentNode.NodeNumber : null,
                IsActive = u.LastLogin > DateTime.UtcNow.AddDays(-7)
            })
            .OrderByDescending(u => u.LastLogin)
            .ToListAsync();

        return Ok(users);
    }

    [HttpDelete("clear-all-users")]
    public async Task<ActionResult> ClearAllUsers()
    {
        var userCount = await _context.Users.CountAsync();
        
        _context.Users.RemoveRange(_context.Users);
        await _context.SaveChangesAsync();

        return Ok(new { Message = $"Cleared {userCount} users from database" });
    }

    [HttpGet("stats")]
    public async Task<ActionResult<AdminStatsDto>> GetAdminStats()
    {
        var stats = new AdminStatsDto
        {
            TotalUsers = await _context.Users.CountAsync(),
            ActiveUsers = await _context.Users.CountAsync(u => u.LastLogin > DateTime.UtcNow.AddDays(-7)),
            TotalRealms = await _context.NexusRealms.CountAsync(),
            TotalNodes = await _context.NeuralNodes.CountAsync(),
            TotalHyperTunnels = await _context.HyperTunnels.CountAsync(),
            UsersWithRealms = await _context.Users.CountAsync(u => u.RealmId.HasValue),
            UsersWithoutRealms = await _context.Users.CountAsync(u => !u.RealmId.HasValue)
        };

        return Ok(stats);
    }

    // Node, Planet, and Starbase Management
    [HttpGet("nodes/{nodeId}")]
    public async Task<ActionResult<NodeDetailAdminDto>> GetNodeDetails(int nodeId)
    {
        var node = await _context.NeuralNodes
            .Include(n => n.Realm)
            .Include(n => n.Planets)
                .ThenInclude(p => p.Owner)
            .Include(n => n.Starbase)
                .ThenInclude(s => s!.Owner)
            .Include(n => n.OutgoingTunnels)
                .ThenInclude(t => t.ToNode)
            .FirstOrDefaultAsync(n => n.Id == nodeId);

        if (node == null)
            return NotFound("Neural Node not found");

        var nodeDetail = new NodeDetailAdminDto
        {
            Id = node.Id,
            NodeNumber = node.NodeNumber,
            RealmName = node.Realm.Name,
            CoordinateX = node.CoordinateX,
            CoordinateY = node.CoordinateY,
            HasQuantumStation = node.HasQuantumStation,
            StarName = node.StarName,
            PlanetCount = node.PlanetCount,
            Planets = node.Planets.Select(p => new PlanetDto
            {
                Id = p.Id,
                PlanetNumber = p.PlanetNumber,
                Name = p.Name,
                Size = p.Size,
                OwnerId = p.OwnerId,
                OwnerUsername = p.Owner?.Username,
                CreatedAt = p.CreatedAt,
                ClaimedAt = p.ClaimedAt
            }).OrderBy(p => p.PlanetNumber).ToList(),
            Starbase = node.Starbase == null ? null : new StarbaseDto
            {
                Id = node.Starbase.Id,
                OwnerId = node.Starbase.OwnerId,
                OwnerUsername = node.Starbase.Owner?.Username,
                DefenseLevel = node.Starbase.DefenseLevel,
                CreatedAt = node.Starbase.CreatedAt,
                ClaimedAt = node.Starbase.ClaimedAt,
                LastAttack = node.Starbase.LastAttack
            },
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

    [HttpPost("planets")]
    public async Task<ActionResult<AdminActionResult>> AddPlanet(AddPlanetRequest request)
    {
        try
        {
            var planet = await _planetService.AddPlanetToNodeAsync(request.NodeId, request.Name, request.Size);
            
            return new AdminActionResult
            {
                Success = true,
                Message = $"Planet '{request.Name}' added successfully",
                Data = new PlanetDto
                {
                    Id = planet.Id,
                    PlanetNumber = planet.PlanetNumber,
                    Name = planet.Name,
                    Size = planet.Size,
                    CreatedAt = planet.CreatedAt
                }
            };
        }
        catch (Exception ex)
        {
            return BadRequest(new AdminActionResult
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpDelete("planets/{planetId}")]
    public async Task<ActionResult<AdminActionResult>> RemovePlanet(int planetId)
    {
        var success = await _planetService.RemovePlanetAsync(planetId);
        
        if (!success)
        {
            return NotFound(new AdminActionResult
            {
                Success = false,
                Message = "Planet not found"
            });
        }

        return new AdminActionResult
        {
            Success = true,
            Message = "Planet removed successfully"
        };
    }

    [HttpPost("starbases")]
    public async Task<ActionResult<AdminActionResult>> AddStarbase(AddStarbaseRequest request)
    {
        try
        {
            var starbase = await _planetService.AddStarbaseToNodeAsync(request.NodeId);
            
            return new AdminActionResult
            {
                Success = true,
                Message = "Starbase added successfully",
                Data = new StarbaseDto
                {
                    Id = starbase.Id,
                    DefenseLevel = starbase.DefenseLevel,
                    CreatedAt = starbase.CreatedAt
                }
            };
        }
        catch (Exception ex)
        {
            return BadRequest(new AdminActionResult
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpDelete("starbases/node/{nodeId}")]
    public async Task<ActionResult<AdminActionResult>> RemoveStarbase(int nodeId)
    {
        var success = await _planetService.RemoveStarbaseAsync(nodeId);
        
        if (!success)
        {
            return NotFound(new AdminActionResult
            {
                Success = false,
                Message = "Starbase not found"
            });
        }

        return new AdminActionResult
        {
            Success = true,
            Message = "Starbase removed successfully"
        };
    }

    [HttpPost("seed-planets")]
    public async Task<ActionResult<AdminActionResult>> SeedAllPlanets()
    {
        try
        {
            await _planetService.SeedPlanetsForAllRealmsAsync();
            
            return new AdminActionResult
            {
                Success = true,
                Message = "All realms have been seeded with planets and starbases"
            };
        }
        catch (Exception ex)
        {
            return BadRequest(new AdminActionResult
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpGet("realms/{realmId}/nodes")]
    public async Task<ActionResult<List<NodeSummaryDto>>> GetRealmNodes(int realmId)
    {
        var nodes = await _context.NeuralNodes
            .Where(n => n.RealmId == realmId)
            .Include(n => n.Planets)
            .Include(n => n.Starbase)
            .Select(n => new NodeSummaryDto
            {
                Id = n.Id,
                NodeNumber = n.NodeNumber,
                CoordinateX = n.CoordinateX,
                CoordinateY = n.CoordinateY,
                HasQuantumStation = n.HasQuantumStation,
                StarName = n.StarName,
                PlanetCount = n.PlanetCount,
                ActualPlanetCount = n.Planets.Count,
                HasStarbase = n.Starbase != null
            })
            .OrderBy(n => n.NodeNumber)
            .ToListAsync();

        return nodes;
    }
}

// DTOs for admin responses
public class UserAdminDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int CargoHolds { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastLogin { get; set; }
    public int? RealmId { get; set; }
    public string? RealmName { get; set; }
    public int? CurrentNodeId { get; set; }
    public int? CurrentNodeNumber { get; set; }
    public bool IsActive { get; set; }
}

public class UserDetailDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int CargoHolds { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastLogin { get; set; }
    public int? CurrentRealmId { get; set; }
    public string? CurrentRealmName { get; set; }
    public int? CurrentNodeId { get; set; }
    public int? CurrentNodeNumber { get; set; }
    public List<RealmHistoryDto> RealmHistory { get; set; } = new();
}

public class RealmHistoryDto
{
    public int? RealmId { get; set; }
    public string RealmName { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
    public int? CurrentNodeId { get; set; }
    public int? CurrentNodeNumber { get; set; }
    public bool IsCurrentRealm { get; set; }
}

public class RealmAdminDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int NodeCount { get; set; }
    public int QuantumStationSeedRate { get; set; }
    public bool NoDeadNodes { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UserCount { get; set; }
    public int ActiveUserCount { get; set; }
}

public class AdminStatsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalRealms { get; set; }
    public int TotalNodes { get; set; }
    public int TotalHyperTunnels { get; set; }
    public int UsersWithRealms { get; set; }
    public int UsersWithoutRealms { get; set; }
}

public class NodeSummaryDto
{
    public int Id { get; set; }
    public int NodeNumber { get; set; }
    public int CoordinateX { get; set; }
    public int CoordinateY { get; set; }
    public bool HasQuantumStation { get; set; }
    public string StarName { get; set; } = string.Empty;
    public int PlanetCount { get; set; }
    public int ActualPlanetCount { get; set; }
    public bool HasStarbase { get; set; }
}