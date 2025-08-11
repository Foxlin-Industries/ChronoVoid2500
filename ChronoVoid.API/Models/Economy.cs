namespace ChronoVoid.API.Models;

public class TradeGood
{
    public int Id { get; set; }
    public int StarbaseId { get; set; }
    public string ResourceType { get; set; } = string.Empty; // Steel, Plasmon, etc.
    public decimal BuyPrice { get; set; } // Price to buy from starbase
    public decimal SellPrice { get; set; } // Price starbase buys for
    public int Stock { get; set; }
    public DateTime LastUpdate { get; set; } = DateTime.UtcNow;

    public Starbase Starbase { get; set; } = null!;
}

public class TradeTransaction
{
    public int Id { get; set; }
    public int StarbaseId { get; set; }
    public int? BuyerId { get; set; }
    public int? SellerId { get; set; }
    public string ResourceType { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public Starbase Starbase { get; set; } = null!;
    public User? Buyer { get; set; }
    public User? Seller { get; set; }
}

public class PlanetProduction
{
    public int Id { get; set; }
    public int PlanetId { get; set; }
    public string ResourceType { get; set; } = string.Empty;
    public decimal BaseRate { get; set; }
    public int CurrentStock { get; set; }
    public DateTime LastUpdate { get; set; } = DateTime.UtcNow;

    public Planet Planet { get; set; } = null!;
}

public class PlanetContract
{
    public int Id { get; set; }
    public int PlanetId { get; set; }
    public int? StarbaseId { get; set; } // Where the contract is sold
    public decimal Price { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }

    public Planet Planet { get; set; } = null!;
    public Starbase? Starbase { get; set; }
}

public class OwnershipLog
{
    public int Id { get; set; }
    public int PlanetId { get; set; }
    public int? PreviousOwnerId { get; set; }
    public int? NewOwnerId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Method { get; set; } = string.Empty; // claim/raid/trade

    public Planet Planet { get; set; } = null!;
    public User? PreviousOwner { get; set; }
    public User? NewOwner { get; set; }
}
