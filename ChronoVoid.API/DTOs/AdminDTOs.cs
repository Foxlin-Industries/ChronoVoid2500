using ChronoVoid.API.Models;

namespace ChronoVoid.API.DTOs;

public class NodeDetailAdminDto
{
    public int Id { get; set; }
    public int NodeNumber { get; set; }
    public string RealmName { get; set; } = string.Empty;
    public int CoordinateX { get; set; }
    public int CoordinateY { get; set; }
    public bool HasQuantumStation { get; set; }
    public string StarName { get; set; } = string.Empty;
    public int PlanetCount { get; set; }
    public List<PlanetDto> Planets { get; set; } = [];
    public StarbaseDto? Starbase { get; set; }
    public List<ConnectedNodeDto> ConnectedNodes { get; set; } = [];
}

public class PlanetDto
{
    public int Id { get; set; }
    public int PlanetNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public PlanetSize Size { get; set; }
    public int? OwnerId { get; set; }
    public string? OwnerUsername { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ClaimedAt { get; set; }
}

public class StarbaseDto
{
    public int Id { get; set; }
    public int? OwnerId { get; set; }
    public string? OwnerUsername { get; set; }
    public int DefenseLevel { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ClaimedAt { get; set; }
    public DateTime? LastAttack { get; set; }
}

public class AddPlanetRequest
{
    public int NodeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public PlanetSize Size { get; set; }
}

public class AddStarbaseRequest
{
    public int NodeId { get; set; }
}

public class AdminActionResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
}