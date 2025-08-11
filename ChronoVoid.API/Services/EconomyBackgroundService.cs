using Microsoft.Extensions.Hosting;

namespace ChronoVoid.API.Services;

public class EconomyBackgroundService : BackgroundService
{
    private readonly ILogger<EconomyBackgroundService> _logger;
    private readonly IServiceProvider _services;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(30);

    public EconomyBackgroundService(ILogger<EconomyBackgroundService> logger, IServiceProvider services)
    {
        _logger = logger;
        _services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Economy background service started");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _services.CreateScope();
                var economy = scope.ServiceProvider.GetRequiredService<EconomyService>();
                var updated = await economy.ProcessProductionTickAsync();
                _logger.LogDebug("Production tick processed. Updated items: {Count}", updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Economy tick failed");
            }

            try { await Task.Delay(_interval, stoppingToken); } catch { }
        }
    }
}
