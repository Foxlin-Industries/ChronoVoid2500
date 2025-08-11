namespace ChronoVoid.API.DTOs;

public class ShipDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Tier { get; set; }
    public int CargoCapacity { get; set; }
    public int WeaponLevel { get; set; }
    public int ShieldLevel { get; set; }
    public int ComputerLevel { get; set; }
    public int LivesRemaining { get; set; }
    public int? CurrentNodeId { get; set; }
}

public class CreateShipRequest
{
    public int OwnerId { get; set; }
    public int Tier { get; set; }
    public string? Name { get; set; }
}

public class UpgradeShipRequest
{
    public int ShipId { get; set; }
    public string Component { get; set; } = string.Empty; // cargo, weapon, shield, computer
    public int Amount { get; set; }
}
