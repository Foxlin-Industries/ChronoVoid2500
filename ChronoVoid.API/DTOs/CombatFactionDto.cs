namespace ChronoVoid.API.DTOs;

public class RecruitTroopsRequest
{
    public int UserId { get; set; }
    public int PlanetId { get; set; }
    public int Level { get; set; }
    public int Count { get; set; }
}

public class RaidRequest
{
    public int AttackerUserId { get; set; }
    public int TargetPlanetId { get; set; }
}

public class RaidResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int AttackerLosses { get; set; }
    public int DefenderLosses { get; set; }
}

public class FactionRequest
{
    public string Name { get; set; } = string.Empty;
}
