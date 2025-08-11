namespace ChronoVoid.API.DTOs;

public class NodeDetailDto
{
    public int Id { get; set; }
    public int NodeNumber { get; set; }
    public required string RealmName { get; set; }
    public bool HasQuantumStation { get; set; }
    public required string StarName { get; set; }
    public int PlanetCount { get; set; }
    public List<ConnectedNodeDto> ConnectedNodes { get; set; } = [];
}

public class ConnectedNodeDto
{
    public int NodeId { get; set; }
    public int NodeNumber { get; set; }
    public bool HasQuantumStation { get; set; }
    public string StarName { get; set; } = string.Empty;
    public int PlanetCount { get; set; }
}

public class MoveRequestDto
{
    public int UserId { get; set; }
    public int FromNodeId { get; set; }
    public int ToNodeId { get; set; }
}

public class NavigationResultDto
{
    public bool Success { get; set; }
    public required string Message { get; set; }
    public required NodeDetailDto CurrentNode { get; set; }
    public DateTime Timestamp { get; set; }
}

public class UserLocationDto
{
    public int UserId { get; set; }
    public required string Username { get; set; }
    public int CurrentNodeId { get; set; }
    public int CurrentNodeNumber { get; set; }
    public int RealmId { get; set; }
    public required string RealmName { get; set; }
    public DateTime LastLogin { get; set; }
}