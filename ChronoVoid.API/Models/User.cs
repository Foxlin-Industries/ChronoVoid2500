namespace ChronoVoid.API.Models;

public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public int? CurrentNodeId { get; set; }
    public int? RealmId { get; set; }
    public int CargoHolds { get; set; } = 300; // Default cargo holds for new players
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastLogin { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public NeuralNode? CurrentNode { get; set; }
    public NexusRealm? Realm { get; set; }
    public ICollection<Planet> OwnedPlanets { get; set; } = [];
    public ICollection<Starbase> OwnedStarbases { get; set; } = [];
}