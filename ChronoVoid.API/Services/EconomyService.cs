using ChronoVoid.API.Data;
using ChronoVoid.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ChronoVoid.API.Services;

public class EconomyService
{
    private static readonly string[] Goods = new[] { "Steel", "Plasmon", "Food Packs", "Gold", "Sealant", "Oxygen Tanks" };
    private readonly ChronoVoidContext _context;
    private readonly ILogger<EconomyService> _logger;

    public EconomyService(ChronoVoidContext context, ILogger<EconomyService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task EnsureMarketSeedAsync(int starbaseId)
    {
        var existing = await _context.TradeGoods.Where(g => g.StarbaseId == starbaseId).ToListAsync();
        if (existing.Count > 0) return;

        var rand = Random.Shared;
        foreach (var g in Goods)
        {
            _context.TradeGoods.Add(new TradeGood
            {
                StarbaseId = starbaseId,
                ResourceType = g,
                BuyPrice = 50 + rand.Next(0, 50),
                SellPrice = 40 + rand.Next(0, 40),
                Stock = 100 + rand.Next(0, 200)
            });
        }
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded market for starbase {StarbaseId}", starbaseId);
    }

    public async Task RecalculateMarketAsync(int starbaseId)
    {
        // Simple placeholder: small random walk for prices based on stock
        var items = await _context.TradeGoods.Where(g => g.StarbaseId == starbaseId).ToListAsync();
        foreach (var item in items)
        {
            var pressure = Math.Clamp(100 - item.Stock, -100, 100); // negative if high stock
            var delta = pressure * 0.01m; // scale
            item.BuyPrice = Math.Max(1, item.BuyPrice + delta);
            item.SellPrice = Math.Max(1, Math.Min(item.BuyPrice - 5, item.SellPrice + delta * 0.8m));
            item.LastUpdate = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();
    }

    public async Task<int> ProcessProductionTickAsync()
    {
        var planets = await _context.Planets.AsNoTracking().ToListAsync();
        int updates = 0;
        foreach (var planet in planets)
        {
            // Ensure production rows exist
            var prod = await _context.PlanetProductions.Where(p => p.PlanetId == planet.Id).ToListAsync();
            if (prod.Count == 0)
            {
                var baseRate = planet.Size switch { PlanetSize.Tiny => 1m, PlanetSize.Average => 3m, PlanetSize.Huge => 6m, _ => 2m };
                foreach (var g in Goods)
                {
                    _context.PlanetProductions.Add(new PlanetProduction
                    {
                        PlanetId = planet.Id,
                        ResourceType = g,
                        BaseRate = baseRate,
                        CurrentStock = 0,
                        LastUpdate = DateTime.UtcNow
                    });
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                foreach (var p in prod)
                {
                    p.CurrentStock += (int)Math.Max(1, Math.Round(p.BaseRate));
                    p.LastUpdate = DateTime.UtcNow;
                    updates++;
                }
            }
        }
        await _context.SaveChangesAsync();
        return updates;
    }
}
