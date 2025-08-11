using System.ComponentModel.DataAnnotations;

namespace ChronoVoid.API.DTOs;

public class AlienRaceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TechnologyLevel { get; set; }
    public bool TranslatorCapable { get; set; }
    public string Disposition { get; set; } = string.Empty;
    public int HumanAgreeability { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? AdditionalTraits { get; set; }
    public string InteractionRules { get; set; } = string.Empty;
}

public class GenerateRacesRequestDto
{
    [Range(1, 10000)]
    public int Count { get; set; } = 5000;
    
    public int? Seed { get; set; }
    
    [Range(1, 10)]
    public int? MinTechnologyLevel { get; set; }
    
    [Range(1, 10)]
    public int? MaxTechnologyLevel { get; set; }
}

public class ActivateRacesRequestDto
{
    [Required]
    [MinLength(1)]
    [MaxLength(50)]
    public List<int> RaceIds { get; set; } = new();
}

public class RaceFilterDto
{
    [Range(1, 10)]
    public int? MinTechnologyLevel { get; set; }
    
    [Range(1, 10)]
    public int? MaxTechnologyLevel { get; set; }
    
    public string? Disposition { get; set; }
    
    [Range(1, 10)]
    public int? MinHumanAgreeability { get; set; }
    
    [Range(1, 10)]
    public int? MaxHumanAgreeability { get; set; }
    
    public bool? TranslatorCapable { get; set; }
    
    public bool? IsActive { get; set; }
    
    [Range(1, 1000)]
    public int PageSize { get; set; } = 50;
    
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;
}

public class RaceStatsDto
{
    public int TotalRaces { get; set; }
    public int ActiveRaces { get; set; }
    public int InactiveRaces { get; set; }
    public Dictionary<string, int> DispositionCounts { get; set; } = new();
    public Dictionary<int, int> TechnologyLevelCounts { get; set; } = new();
    public Dictionary<int, int> AgreeabilityCounts { get; set; } = new();
    public int TranslatorCapableCount { get; set; }
    public int NonTranslatorCapableCount { get; set; }
}