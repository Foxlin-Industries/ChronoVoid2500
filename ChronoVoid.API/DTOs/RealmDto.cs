namespace ChronoVoid.API.DTOs;

public class RealmDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int NodeCount { get; set; }
    public int QuantumStationSeedRate { get; set; }
    public bool NoDeadNodes { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class CreateRealmRequest
{
    public required string Name { get; set; }
    public int NodeCount { get; set; }
    public int QuantumStationSeedRate { get; set; }
    public bool NoDeadNodes { get; set; }
}