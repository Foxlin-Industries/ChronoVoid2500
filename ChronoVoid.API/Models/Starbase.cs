namespace ChronoVoid.API.Models;

public class Starbase
{
    public int Id { get; set; }
    public int NodeId { get; set; }
    public int? OwnerId { get; set; } // User who owns this starbase
    public int DefenseLevel { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ClaimedAt { get; set; }
    public DateTime? LastAttack { get; set; }
    
    // Navigation properties
    public NeuralNode Node { get; set; } = null!;
    public User? Owner { get; set; }
}