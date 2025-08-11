using ChronoVoid.API.Data;
using ChronoVoid.API.DTOs;
using ChronoVoid.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronoVoid.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CombatController : ControllerBase
{
    private readonly ChronoVoidContext _context;

    public CombatController(ChronoVoidContext context)
    {
        _context = context;
    }

    [HttpPost("recruit")]
    public async Task<ActionResult> Recruit(RecruitTroopsRequest request)
    {
        var user = await _context.Users.FindAsync(request.UserId);
        var planet = await _context.Planets.FindAsync(request.PlanetId);
        if (user == null || planet == null) return BadRequest("Invalid user or planet");

        var troop = new Troop
        {
            OwnerId = user.Id,
            PlanetId = planet.Id,
            Level = Math.Clamp(request.Level, 1, 10),
            Count = Math.Max(1, request.Count),
            DailyPay = 3m * request.Level * (planet.Size == PlanetSize.Huge ? 3 : planet.Size == PlanetSize.Average ? 2 : 1)
        };

        _context.Troops.Add(troop);
        await _context.SaveChangesAsync();
        return Ok(new { Message = "Troops recruited" });
    }

    [HttpPost("raid")]
    public async Task<ActionResult<RaidResultDto>> Raid(RaidRequest request)
    {
        var attacker = await _context.Users.FindAsync(request.AttackerUserId);
        if (attacker == null) return BadRequest("Attacker not found");
        var planet = await _context.Planets.Include(p => p.Owner).FirstOrDefaultAsync(p => p.Id == request.TargetPlanetId);
        if (planet == null) return BadRequest("Planet not found");

        // Simplified MVP resolution
        var rand = Random.Shared;
        bool success = rand.NextDouble() < 0.2; // 20% base chance
        int attackerLosses = rand.Next(0, 5);
        int defenderLosses = rand.Next(0, 5);

        if (success)
        {
            int? prevOwner = planet.OwnerId;
            planet.OwnerId = attacker.Id;
            await _context.SaveChangesAsync();
            _context.OwnershipLogs.Add(new OwnershipLog
            {
                PlanetId = planet.Id,
                PreviousOwnerId = prevOwner,
                NewOwnerId = attacker.Id,
                Method = "raid"
            });
            await _context.SaveChangesAsync();
        }

        return Ok(new RaidResultDto
        {
            Success = success,
            Message = success ? "Raid successful. Ownership transferred." : "Raid failed.",
            AttackerLosses = attackerLosses,
            DefenderLosses = defenderLosses
        });
    }
}
