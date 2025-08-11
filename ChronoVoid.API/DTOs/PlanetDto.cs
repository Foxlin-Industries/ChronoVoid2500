namespace ChronoVoid.API.DTOs;

public class PlanetSummaryDto
{
    public int Id { get; set; }
    public int NodeId { get; set; }
    public int PlanetNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public int? OwnerId { get; set; }
}

public class ClaimPlanetRequest
{
    public int UserId { get; set; }
    public int PlanetId { get; set; }
    public int? ContractId { get; set; }
    public decimal? Credits { get; set; }
}

public class ProductionDto
{
    public string ResourceType { get; set; } = string.Empty;
    public decimal BaseRate { get; set; }
    public int CurrentStock { get; set; }
}
