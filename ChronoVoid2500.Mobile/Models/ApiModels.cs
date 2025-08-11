namespace ChronoVoid2500.Mobile.Models;

public class LoginRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class RegisterRequest
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class AuthResponse
{
    public int UserId { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public int? CurrentNodeId { get; set; }
    public int? RealmId { get; set; }
    public int CargoHolds { get; set; }
    public DateTime LastLogin { get; set; }
    public required string Token { get; set; }
}

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

public class JoinRealmRequest
{
    public int UserId { get; set; }
    public int RealmId { get; set; }
}

public class JoinRealmResponse
{
    public bool Success { get; set; }
    public required string Message { get; set; }
    public int? StartingNodeId { get; set; }
    public NodeDetailDto? StartingNode { get; set; }
}

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