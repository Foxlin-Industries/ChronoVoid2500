using System.ComponentModel.DataAnnotations;

namespace ChronoVoid.API.DTOs;

public class RealmCreationDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;
    
    [Range(100, 100000)]
    public int SystemCount { get; set; } = 10000;
    
    [Range(4, 8)]
    public int MinHyperTunnels { get; set; } = 4;
    
    [Range(4, 8)]
    public int MaxHyperTunnels { get; set; } = 8;
    
    // Planet Generation Settings
    [Range(1, 9)]
    public int MinPlanetsPerSystem { get; set; } = 1;
    
    [Range(1, 9)]
    public int MaxPlanetsPerSystem { get; set; } = 9;
    
    public PlanetDensity PlanetDensity { get; set; } = PlanetDensity.Default;
    
    // Resource Settings
    public ResourceDensity ResourceDensity { get; set; } = ResourceDensity.Medium;
    
    // Quantum Station Settings
    [Range(0.1, 1.0)]
    public double QuantumStationChance { get; set; } = 0.3;
    
    [Range(3, 9)]
    public int MinPlanetsForQuantumStation { get; set; } = 3;
    
    // Alien Race Settings
    public bool IncludeAlienRaces { get; set; } = true;
    
    [Range(10, 50)]
    public int MaxActiveAlienRaces { get; set; } = 50;
    
    [Range(1, 10)]
    public int? MinAlienTechLevel { get; set; }
    
    [Range(1, 10)]
    public int? MaxAlienTechLevel { get; set; }
    
    // Planet Purchase Contract Settings
    [Range(2, 20)]
    public int PlanetPurchaseContracts { get; set; } = 5;
    
    // Advanced Settings
    public bool EnableArtifactSystems { get; set; } = true;
    
    [Range(0.01, 0.1)]
    public double ArtifactSystemChance { get; set; } = 0.05;
    
    public string? Description { get; set; }
}

public enum PlanetDensity
{
    Sparse = 1,     // 20% fewer planets
    Default = 2,    // Normal distribution
    Dense = 3,      // 50% more planets
    VeryDense = 4   // 100% more planets
}

public enum ResourceDensity
{
    VeryLight = 1,  // 25% of normal resources
    Light = 2,      // 50% of normal resources
    Medium = 3,     // Normal resource generation
    Heavy = 4,      // 150% of normal resources
    VeryHeavy = 5,  // 200% of normal resources
    Abundant = 6    // 300% of normal resources
}

public class RealmCreationResultDto
{
    public int RealmId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SystemsCreated { get; set; }
    public int PlanetsCreated { get; set; }
    public int QuantumStationsCreated { get; set; }
    public int HyperTunnelsCreated { get; set; }
    public int ActiveAlienRaces { get; set; }
    public int ArtifactSystemsCreated { get; set; }
    public TimeSpan GenerationTime { get; set; }
    public string Status { get; set; } = "Success";
    public List<string> Warnings { get; set; } = new();
}

public class RealmStatsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SystemCount { get; set; }
    public int PlanetCount { get; set; }
    public int QuantumStationCount { get; set; }
    public int HyperTunnelCount { get; set; }
    public int UserCount { get; set; }
    public int ActiveUserCount { get; set; }
    public int ActiveAlienRaces { get; set; }
    public DateTime CreatedAt { get; set; }
    public PlanetDensity PlanetDensity { get; set; }
    public ResourceDensity ResourceDensity { get; set; }
    public string? Description { get; set; }
}