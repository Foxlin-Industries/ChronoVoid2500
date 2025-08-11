namespace ChronoVoid.API.DTOs;

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
    public required string Token { get; set; } // For future JWT implementation
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

public class ForgotPasswordRequest
{
    public required string EmailOrUsername { get; set; }
}