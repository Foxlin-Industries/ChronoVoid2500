namespace ChronoVoid.API.Models;

public class HyperTunnel
{
    public int Id { get; set; }
    public int FromNodeId { get; set; }
    public int ToNodeId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public NeuralNode FromNode { get; set; } = null!;
    public NeuralNode ToNode { get; set; } = null!;
}