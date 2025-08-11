using ChronoVoid.API.Data;
using ChronoVoid.API.DTOs;
using ChronoVoid.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronoVoid.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShipController : ControllerBase
{
    private readonly ChronoVoidContext _context;

    public ShipController(ChronoVoidContext context)
    {
        _context = context;
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<ShipDto>>> GetUserShips(int userId)
    {
        var ships = await _context.Ships
            .Where(s => s.OwnerId == userId)
            .Select(s => new ShipDto
            {
                Id = s.Id,
                Name = s.Name,
                Tier = s.Tier,
                CargoCapacity = s.CargoCapacity,
                WeaponLevel = s.WeaponLevel,
                ShieldLevel = s.ShieldLevel,
                ComputerLevel = s.ComputerLevel,
                LivesRemaining = s.LivesRemaining,
                CurrentNodeId = s.CurrentNodeId
            })
            .ToListAsync();

        return Ok(ships);
    }

    [HttpPost]
    public async Task<ActionResult<ShipDto>> CreateShip(CreateShipRequest request)
    {
        var owner = await _context.Users.FindAsync(request.OwnerId);
        if (owner == null) return NotFound("Owner not found");

        // Simple defaults for tier
        var tier = Math.Clamp(request.Tier, 1, 30);
        var ship = new Ship
        {
            OwnerId = owner.Id,
            Tier = tier,
            Name = string.IsNullOrWhiteSpace(request.Name) ? $"Tier {tier} Ship" : request.Name!,
            CargoCapacity = tier == 1 ? 1 : 100 + (tier * 50),
            WeaponLevel = Math.Max(0, tier - 1),
            ShieldLevel = Math.Max(0, tier - 1),
            ComputerLevel = Math.Max(1, tier / 2),
            LivesRemaining = tier == 1 ? 10 : 0,
            CurrentNodeId = owner.CurrentNodeId
        };

        _context.Ships.Add(ship);
        await _context.SaveChangesAsync();

        var dto = new ShipDto
        {
            Id = ship.Id,
            Name = ship.Name,
            Tier = ship.Tier,
            CargoCapacity = ship.CargoCapacity,
            WeaponLevel = ship.WeaponLevel,
            ShieldLevel = ship.ShieldLevel,
            ComputerLevel = ship.ComputerLevel,
            LivesRemaining = ship.LivesRemaining,
            CurrentNodeId = ship.CurrentNodeId
        };
        return CreatedAtAction(nameof(GetUserShips), new { userId = owner.Id }, dto);
    }

    [HttpPost("upgrade")]
    public async Task<ActionResult<ShipDto>> Upgrade(UpgradeShipRequest request)
    {
        var ship = await _context.Ships.FindAsync(request.ShipId);
        if (ship == null) return NotFound("Ship not found");

        switch (request.Component.ToLowerInvariant())
        {
            case "cargo":
                ship.CargoCapacity += request.Amount;
                break;
            case "weapon":
                ship.WeaponLevel += request.Amount;
                break;
            case "shield":
                ship.ShieldLevel += request.Amount;
                break;
            case "computer":
                ship.ComputerLevel += request.Amount;
                break;
            default:
                return BadRequest("Unknown component. Use cargo|weapon|shield|computer");
        }

        await _context.SaveChangesAsync();

        return Ok(new ShipDto
        {
            Id = ship.Id,
            Name = ship.Name,
            Tier = ship.Tier,
            CargoCapacity = ship.CargoCapacity,
            WeaponLevel = ship.WeaponLevel,
            ShieldLevel = ship.ShieldLevel,
            ComputerLevel = ship.ComputerLevel,
            LivesRemaining = ship.LivesRemaining,
            CurrentNodeId = ship.CurrentNodeId
        });
    }
}
