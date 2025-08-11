namespace ChronoVoid.API.Models;

public class Troop
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public int PlanetId { get; set; }
    public int Level { get; set; } // 1..10
    public int Count { get; set; }
    public DateTime DeployedAt { get; set; } = DateTime.UtcNow;
    public decimal DailyPay { get; set; }

    public User Owner { get; set; } = null!;
    public Planet Planet { get; set; } = null!;
}

public class Faction
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? LeaderId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int XP { get; set; }
    public string? WarMode { get; set; }

    public ICollection<FactionMember> Members { get; set; } = [];
}

public class FactionMember
{
    public int Id { get; set; }
    public int FactionId { get; set; }
    public int UserId { get; set; }
    public string Role { get; set; } = "Member";
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public Faction Faction { get; set; } = null!;
    public User User { get; set; } = null!;
}
