namespace ChronoVoid.API.Models;

public class Ship
{
    public int Id { get; set; }
    public required string Name { get; set; } // Tier name e.g., "Escape Pod", "Freighter"
    public int OwnerId { get; set; }
    public int? CurrentNodeId { get; set; }
    public int Tier { get; set; } // 1..30
    public int CargoCapacity { get; set; }
    public int WeaponLevel { get; set; }
    public int ShieldLevel { get; set; }
    public int ComputerLevel { get; set; }
    public int LivesRemaining { get; set; } = 10; // For Escape Pod logic
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User Owner { get; set; } = null!;
    public NeuralNode? CurrentNode { get; set; }
    public ICollection<ShipCargo> Cargo { get; set; } = [];
}

public class ShipCargo
{
    public int Id { get; set; }
    public int ShipId { get; set; }
    public string ResourceType { get; set; } = string.Empty; // Steel, Plasmon, etc.
    public int Quantity { get; set; }

    public Ship Ship { get; set; } = null!;
}
