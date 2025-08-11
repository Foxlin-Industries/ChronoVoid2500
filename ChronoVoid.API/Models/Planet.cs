namespace ChronoVoid.API.Models;

public class Planet
{
    public int Id { get; set; }
    public int NodeId { get; set; }
    public int PlanetNumber { get; set; } // 1-9 for the solar system planets
    public required string Name { get; set; } // Mercury, Venus, Earth, Mars, Jupiter, Saturn, Uranus, Neptune, Pluto
    public PlanetSize Size { get; set; }
    public int? OwnerId { get; set; } // User who owns this planet
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ClaimedAt { get; set; }
    
    // Navigation properties
    public NeuralNode Node { get; set; } = null!;
    public User? Owner { get; set; }
}

public enum PlanetSize
{
    Tiny = 1,
    Average = 2,
    Huge = 3
}