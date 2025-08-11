using ChronoVoid.API.Data;
using ChronoVoid.API.DTOs;
using ChronoVoid.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronoVoid.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FactionController : ControllerBase
{
    private readonly ChronoVoidContext _context;

    public FactionController(ChronoVoidContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult> Create(FactionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name)) return BadRequest("Name required");
        if (await _context.Factions.AnyAsync(f => f.Name.ToLower() == request.Name.ToLower()))
            return BadRequest("Faction name already exists");

        var faction = new Faction { Name = request.Name };
        _context.Factions.Add(faction);
        await _context.SaveChangesAsync();
        return Ok(new { faction.Id, faction.Name });
    }

    [HttpPost("{factionId}/join/{userId}")]
    public async Task<ActionResult> Join(int factionId, int userId)
    {
        var faction = await _context.Factions.FindAsync(factionId);
        var user = await _context.Users.FindAsync(userId);
        if (faction == null || user == null) return NotFound();

        if (await _context.FactionMembers.AnyAsync(m => m.FactionId == factionId && m.UserId == userId))
            return BadRequest("Already a member");

        _context.FactionMembers.Add(new FactionMember { FactionId = factionId, UserId = userId });
        await _context.SaveChangesAsync();
        return Ok(new { Message = "Joined faction" });
    }

    [HttpGet("{factionId}")]
    public async Task<ActionResult> Get(int factionId)
    {
        var faction = await _context.Factions
            .Include(f => f.Members)
            .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(f => f.Id == factionId);
        if (faction == null) return NotFound();

        return Ok(new
        {
            faction.Id,
            faction.Name,
            Members = faction.Members.Select(m => new { m.UserId, m.Role, m.JoinedAt })
        });
    }
}
