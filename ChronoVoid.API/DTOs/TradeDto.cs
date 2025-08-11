namespace ChronoVoid.API.DTOs;

public class MarketItemDto
{
    public int StarbaseId { get; set; }
    public string ResourceType { get; set; } = string.Empty;
    public decimal BuyPrice { get; set; }
    public decimal SellPrice { get; set; }
    public int Stock { get; set; }
}

public class MarketDto
{
    public int StarbaseId { get; set; }
    public string StarName { get; set; } = string.Empty;
    public List<MarketItemDto> Items { get; set; } = [];
}

public class TradeRequestDto
{
    public int UserId { get; set; }
    public int StarbaseId { get; set; }
    public string ResourceType { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public bool IsBuy { get; set; }
}

public class TradeResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public MarketItemDto? UpdatedItem { get; set; }
}
