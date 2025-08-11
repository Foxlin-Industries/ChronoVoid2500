using ChronoVoid.API.Data;
using ChronoVoid.API.DTOs;
using ChronoVoid.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronoVoid.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlanetController : ControllerBase
{
    private readonly ChronoVoidContext _context;

    public PlanetController(ChronoVoidContext context)
    {
        _context = context;
    }

    [HttpGet("node/{nodeId}")]
    public async Task<ActionResult<IEnumerable<PlanetSummaryDto>>> GetPlanetsInNode(int nodeId)
    {
        var planets = await _context.Planets
            .Where(p => p.NodeId == nodeId)
            .Select(p => new PlanetSummaryDto
            {
                Id = p.Id,
                NodeId = p.NodeId,
                PlanetNumber = p.PlanetNumber,
                Name = p.Name,
                Size = p.Size.ToString(),
                OwnerId = p.OwnerId
            })
            .ToListAsync();
        return Ok(planets);
    }

    [HttpPost("claim")]
    public async Task<ActionResult> Claim(ClaimPlanetRequest request)
    {
        var user = await _context.Users.FindAsync(request.UserId);
        var planet = await _context.Planets.FindAsync(request.PlanetId);
        if (user == null || planet == null) return BadRequest("Invalid user or planet");

        // Simple rules: allow claim if no owner and either contract or credits provided
        if (planet.OwnerId.HasValue) return BadRequest("Planet already owned");

        planet.OwnerId = user.Id;
        planet.ClaimedAt = DateTime.UtcNow;
        _context.OwnershipLogs.Add(new OwnershipLog
        {
            PlanetId = planet.Id,
            PreviousOwnerId = null,
            NewOwnerId = user.Id,
            Method = request.ContractId.HasValue ? "contract" : "credits"
        });
        await _context.SaveChangesAsync();
        return Ok(new { Message = "Planet claimed" });
    }

    [HttpGet("{planetId}/production")]
    public async Task<ActionResult<IEnumerable<ProductionDto>>> GetProduction(int planetId)
    {
        var items = await _context.PlanetProductions
            .Where(pp => pp.PlanetId == planetId)
            .Select(pp => new ProductionDto
            {
                ResourceType = pp.ResourceType,
                BaseRate = pp.BaseRate,
                CurrentStock = pp.CurrentStock
            })
            .ToListAsync();
        return Ok(items);
    }
}
