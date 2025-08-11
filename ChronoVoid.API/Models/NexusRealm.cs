namespace ChronoVoid.API.Models;

public class NexusRealm
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int NodeCount { get; set; }
    public int QuantumStationSeedRate { get; set; } // Percentage 0-100
    public bool NoDeadNodes { get; set; } // New requirement
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Advanced Realm Settings
    public int PlanetDensity { get; set; } = 2; // Default density
    public int ResourceDensity { get; set; } = 3; // Medium resources
    public int MinPlanetsPerSystem { get; set; } = 1;
    public int MaxPlanetsPerSystem { get; set; } = 9;
    public int PlanetPurchaseContracts { get; set; } = 5;
    public bool EnableArtifactSystems { get; set; } = true;
    public double ArtifactSystemChance { get; set; } = 0.05;
    public int ActiveAlienRaces { get; set; } = 0;
    public int? MinAlienTechLevel { get; set; }
    public int? MaxAlienTechLevel { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public ICollection<NeuralNode> Nodes { get; set; } = [];
    public ICollection<User> Users { get; set; } = [];
}