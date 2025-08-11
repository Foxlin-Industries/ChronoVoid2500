using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChronoVoid.API.Data;
using ChronoVoid.API.Models;
using ChronoVoid.API.DTOs;
using ChronoVoid.API.Services;

namespace ChronoVoid.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlienRaceController : ControllerBase
{
    private readonly ChronoVoidContext _context;
    private readonly ILogger<AlienRaceController> _logger;

    public AlienRaceController(ChronoVoidContext context, ILogger<AlienRaceController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Generate alien races and store them in the database
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<List<AlienRaceDto>>> GenerateRaces([FromBody] GenerateRacesRequestDto request)
    {
        try
        {
            _logger.LogInformation("Generating {Count} alien races with seed {Seed}", request.Count, request.Seed);

            var generator = new AlienRaceGeneratorService(request.Seed);
            var races = generator.GenerateRaces(request.Count);

            // Apply technology level filters if specified
            if (request.MinTechnologyLevel.HasValue || request.MaxTechnologyLevel.HasValue)
            {
                races = races.Where(r => 
                    (!request.MinTechnologyLevel.HasValue || r.TechnologyLevel >= request.MinTechnologyLevel.Value) &&
                    (!request.MaxTechnologyLevel.HasValue || r.TechnologyLevel <= request.MaxTechnologyLevel.Value)
                ).ToList();
            }

            // Clear existing races if generating new set
            var existingRaces = await _context.AlienRaces.ToListAsync();
            _context.AlienRaces.RemoveRange(existingRaces);

            // Add new races
            _context.AlienRaces.AddRange(races);
            await _context.SaveChangesAsync();

            var raceDtos = races.Select(MapToDto).ToList();
            
            _logger.LogInformation("Successfully generated and stored {Count} alien races", raceDtos.Count);
            return Ok(raceDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating alien races");
            return StatusCode(500, "Internal server error while generating races");
        }
    }

    /// <summary>
    /// Get all alien races with optional filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<AlienRaceDto>>> GetRaces([FromQuery] RaceFilterDto filter)
    {
        try
        {
            var query = _context.AlienRaces.AsQueryable();

            // Apply filters
            if (filter.MinTechnologyLevel.HasValue)
                query = query.Where(r => r.TechnologyLevel >= filter.MinTechnologyLevel.Value);

            if (filter.MaxTechnologyLevel.HasValue)
                query = query.Where(r => r.TechnologyLevel <= filter.MaxTechnologyLevel.Value);

            if (!string.IsNullOrEmpty(filter.Disposition))
                query = query.Where(r => r.Disposition == filter.Disposition);

            if (filter.MinHumanAgreeability.HasValue)
                query = query.Where(r => r.HumanAgreeability >= filter.MinHumanAgreeability.Value);

            if (filter.MaxHumanAgreeability.HasValue)
                query = query.Where(r => r.HumanAgreeability <= filter.MaxHumanAgreeability.Value);

            if (filter.TranslatorCapable.HasValue)
                query = query.Where(r => r.TranslatorCapable == filter.TranslatorCapable.Value);

            if (filter.IsActive.HasValue)
                query = query.Where(r => r.IsActive == filter.IsActive.Value);

            // Apply pagination
            var totalCount = await query.CountAsync();
            var races = await query
                .OrderBy(r => r.Name)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var raceDtos = races.Select(MapToDto).ToList();

            Response.Headers.Add("X-Total-Count", totalCount.ToString());
            Response.Headers.Add("X-Page", filter.Page.ToString());
            Response.Headers.Add("X-Page-Size", filter.PageSize.ToString());

            return Ok(raceDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving alien races");
            return StatusCode(500, "Internal server error while retrieving races");
        }
    }

    /// <summary>
    /// Get a specific alien race by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AlienRaceDto>> GetRace(int id)
    {
        try
        {
            var race = await _context.AlienRaces.FindAsync(id);
            if (race == null)
                return NotFound($"Alien race with ID {id} not found");

            return Ok(MapToDto(race));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving alien race {Id}", id);
            return StatusCode(500, "Internal server error while retrieving race");
        }
    }

    /// <summary>
    /// Activate selected races (max 50)
    /// </summary>
    [HttpPost("activate")]
    public async Task<ActionResult> ActivateRaces([FromBody] ActivateRacesRequestDto request)
    {
        try
        {
            if (request.RaceIds.Count > 50)
                return BadRequest("Cannot activate more than 50 races at once");

            // Deactivate all races first
            await _context.AlienRaces.ExecuteUpdateAsync(r => r.SetProperty(p => p.IsActive, false));

            // Activate selected races
            var racesToActivate = await _context.AlienRaces
                .Where(r => request.RaceIds.Contains(r.Id))
                .ToListAsync();

            if (racesToActivate.Count != request.RaceIds.Count)
                return BadRequest("Some race IDs were not found");

            foreach (var race in racesToActivate)
            {
                race.IsActive = true;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Activated {Count} alien races", racesToActivate.Count);
            return Ok(new { Message = $"Successfully activated {racesToActivate.Count} races", ActiveRaces = racesToActivate.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating alien races");
            return StatusCode(500, "Internal server error while activating races");
        }
    }

    /// <summary>
    /// Get random races for admin selection
    /// </summary>
    [HttpGet("random")]
    public async Task<ActionResult<List<AlienRaceDto>>> GetRandomRaces([FromQuery] int count = 50, [FromQuery] int? minTech = null, [FromQuery] int? maxTech = null)
    {
        try
        {
            if (count > 100)
                return BadRequest("Cannot request more than 100 random races at once");

            var query = _context.AlienRaces.Where(r => !r.IsActive);

            // Apply tech level filters
            if (minTech.HasValue)
                query = query.Where(r => r.TechnologyLevel >= minTech.Value);

            if (maxTech.HasValue)
                query = query.Where(r => r.TechnologyLevel <= maxTech.Value);

            var totalAvailable = await query.CountAsync();
            if (totalAvailable < count)
                return BadRequest($"Only {totalAvailable} inactive races available with specified filters");

            // Get random races using GUID ordering (PostgreSQL compatible)
            var randomRaces = await query
                .OrderBy(r => Guid.NewGuid())
                .Take(count)
                .ToListAsync();

            var raceDtos = randomRaces.Select(MapToDto).ToList();
            return Ok(raceDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving random alien races");
            return StatusCode(500, "Internal server error while retrieving random races");
        }
    }

    /// <summary>
    /// Get alien race statistics
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<RaceStatsDto>> GetRaceStats()
    {
        try
        {
            var races = await _context.AlienRaces.ToListAsync();

            var stats = new RaceStatsDto
            {
                TotalRaces = races.Count,
                ActiveRaces = races.Count(r => r.IsActive),
                InactiveRaces = races.Count(r => !r.IsActive),
                DispositionCounts = races.GroupBy(r => r.Disposition).ToDictionary(g => g.Key, g => g.Count()),
                TechnologyLevelCounts = races.GroupBy(r => r.TechnologyLevel).ToDictionary(g => g.Key, g => g.Count()),
                AgreeabilityCounts = races.GroupBy(r => r.HumanAgreeability).ToDictionary(g => g.Key, g => g.Count()),
                TranslatorCapableCount = races.Count(r => r.TranslatorCapable),
                NonTranslatorCapableCount = races.Count(r => !r.TranslatorCapable)
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving alien race statistics");
            return StatusCode(500, "Internal server error while retrieving statistics");
        }
    }

    /// <summary>
    /// Get all available dispositions
    /// </summary>
    [HttpGet("dispositions")]
    public ActionResult<string[]> GetDispositions()
    {
        return Ok(AlienDispositions.All);
    }

    /// <summary>
    /// Remove a race and replace with a random one
    /// </summary>
    [HttpPost("{id}/replace")]
    public async Task<ActionResult<AlienRaceDto>> ReplaceRace(int id, [FromQuery] int? minTech = null, [FromQuery] int? maxTech = null)
    {
        try
        {
            var raceToReplace = await _context.AlienRaces.FindAsync(id);
            if (raceToReplace == null)
                return NotFound($"Race with ID {id} not found");

            if (!raceToReplace.IsActive)
                return BadRequest("Can only replace active races");

            // Find a random inactive race
            var query = _context.AlienRaces.Where(r => !r.IsActive);

            if (minTech.HasValue)
                query = query.Where(r => r.TechnologyLevel >= minTech.Value);

            if (maxTech.HasValue)
                query = query.Where(r => r.TechnologyLevel <= maxTech.Value);

            var replacementRace = await query
                .OrderBy(r => Guid.NewGuid())
                .FirstOrDefaultAsync();

            if (replacementRace == null)
                return BadRequest("No inactive races available for replacement with specified filters");

            // Swap active status
            raceToReplace.IsActive = false;
            replacementRace.IsActive = true;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Replaced race {OldId} ({OldName}) with race {NewId} ({NewName})", 
                id, raceToReplace.Name, replacementRace.Id, replacementRace.Name);

            return Ok(MapToDto(replacementRace));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error replacing alien race {Id}", id);
            return StatusCode(500, "Internal server error while replacing race");
        }
    }

    private static AlienRaceDto MapToDto(AlienRace race)
    {
        return new AlienRaceDto
        {
            Id = race.Id,
            Name = race.Name,
            TechnologyLevel = race.TechnologyLevel,
            TranslatorCapable = race.TranslatorCapable,
            Disposition = race.Disposition,
            HumanAgreeability = race.HumanAgreeability,
            IsActive = race.IsActive,
            CreatedAt = race.CreatedAt,
            AdditionalTraits = race.AdditionalTraits,
            InteractionRules = AlienDispositions.GetInteractionRules(race.Disposition)
        };
    }
}