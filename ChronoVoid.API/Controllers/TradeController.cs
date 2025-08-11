using ChronoVoid.API.Data;
using ChronoVoid.API.DTOs;
using ChronoVoid.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronoVoid.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TradeController : ControllerBase
{
    private readonly ChronoVoidContext _context;
    private readonly Services.EconomyService _economy;
    private readonly Microsoft.AspNetCore.SignalR.IHubContext<ChronoVoid.API.Hubs.TradeHub> _tradeHub;

    public TradeController(ChronoVoidContext context, Services.EconomyService economy, Microsoft.AspNetCore.SignalR.IHubContext<ChronoVoid.API.Hubs.TradeHub> tradeHub)
    {
        _context = context;
        _economy = economy;
        _tradeHub = tradeHub;
    }

    [HttpGet("starbase/by-node/{nodeId}")]
    public async Task<ActionResult<object>> GetStarbaseByNode(int nodeId)
    {
        var starbase = await _context.Starbases.FirstOrDefaultAsync(s => s.NodeId == nodeId);
        if (starbase == null) return NotFound("No starbase in this node");
        return Ok(new { starbase.Id, starbase.NodeId, starbase.OwnerId, starbase.DefenseLevel });
    }

    [HttpGet("starbase/{starbaseId}/market")]
    public async Task<ActionResult<MarketDto>> GetMarket(int starbaseId)
    {
        var starbase = await _context.Starbases
            .Include(s => s.Node)
            .FirstOrDefaultAsync(s => s.Id == starbaseId);
        if (starbase == null) return NotFound("Starbase not found");

        await _economy.EnsureMarketSeedAsync(starbase.Id);

        var goods = await _context.TradeGoods
            .Where(g => g.StarbaseId == starbaseId)
            .ToListAsync();

        var market = new MarketDto
        {
            StarbaseId = starbase.Id,
            StarName = starbase.Node.StarName,
            Items = goods.Select(g => new MarketItemDto
            {
                StarbaseId = g.StarbaseId,
                ResourceType = g.ResourceType,
                BuyPrice = g.BuyPrice,
                SellPrice = g.SellPrice,
                Stock = g.Stock
            }).ToList()
        };
        return Ok(market);
    }

    [HttpPost("trade")]
    public async Task<ActionResult<TradeResultDto>> Trade(TradeRequestDto request)
    {
        var user = await _context.Users.FindAsync(request.UserId);
        var starbase = await _context.Starbases.FindAsync(request.StarbaseId);
        if (user == null || starbase == null) return BadRequest("Invalid user or starbase");

        var item = await _context.TradeGoods.FirstOrDefaultAsync(g => g.StarbaseId == request.StarbaseId && g.ResourceType == request.ResourceType);
        if (item == null) return BadRequest("Resource not traded here");

        if (request.Quantity <= 0) return BadRequest("Quantity must be positive");

        if (request.IsBuy)
        {
            if (item.Stock < request.Quantity)
                return BadRequest("Not enough stock");

            item.Stock -= request.Quantity;
            // In a full implementation, deduct credits and add to cargo
        }
        else
        {
            item.Stock += request.Quantity;
            // In a full implementation, add credits and remove from cargo
        }

        _context.TradeTransactions.Add(new TradeTransaction
        {
            StarbaseId = starbase.Id,
            BuyerId = request.IsBuy ? user.Id : null,
            SellerId = request.IsBuy ? null : user.Id,
            ResourceType = request.ResourceType,
            Quantity = request.Quantity,
            Price = request.IsBuy ? item.BuyPrice : item.SellPrice,
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        // Recalculate market and broadcast update
        await _economy.RecalculateMarketAsync(starbase.Id);
        var market = await GetMarket(starbase.Id);
        if (market.Value != null)
        {
            await _tradeHub.Clients.All.SendAsync("MarketUpdated", market.Value);
        }

        return Ok(new TradeResultDto
        {
            Success = true,
            Message = "Trade executed",
            UpdatedItem = new MarketItemDto
            {
                StarbaseId = item.StarbaseId,
                ResourceType = item.ResourceType,
                BuyPrice = item.BuyPrice,
                SellPrice = item.SellPrice,
                Stock = item.Stock
            }
        });
    }
}
