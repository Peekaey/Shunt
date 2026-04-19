using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shunt.Main.Interfaces;

namespace Shunt.Main.Services;

public class BackgroundWorkerService : BackgroundService
{
    private readonly ILogger<BackgroundWorkerService> _logger;
    private readonly IGameService _gameService;
    public BackgroundWorkerService(ILogger<BackgroundWorkerService> logger ,IGameService gameService)
    {
        _logger = logger;
        _gameService = gameService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background Worker Service is starting...");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Background Worker Service is working...");

            try
            {
                await _gameService.CheckForRunningSteamGame();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while checking for running Steam games.");
            }

            try
            {
                await Task.Delay(5000, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Background Worker Service is stopping...");
            }
        }
        
    }
    
}