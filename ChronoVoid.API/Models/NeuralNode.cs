namespace ChronoVoid.API.Models;

public class NeuralNode
{
    public int Id { get; set; }
    public int RealmId { get; set; }
    public int NodeNumber { get; set; } // 1-based node numbering
    public int CoordinateX { get; set; }
    public int CoordinateY { get; set; }
    public bool HasQuantumStation { get; set; }
    public required string StarName { get; set; }
    public int PlanetCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public NexusRealm Realm { get; set; } = null!;
    public ICollection<HyperTunnel> OutgoingTunnels { get; set; } = [];
    public ICollection<HyperTunnel> IncomingTunnels { get; set; } = [];
    public ICollection<User> CurrentUsers { get; set; } = [];
    public ICollection<Planet> Planets { get; set; } = [];
    public Starbase? Starbase { get; set; }
}