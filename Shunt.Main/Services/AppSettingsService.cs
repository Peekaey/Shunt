using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shunt.Main.Interfaces;
using Shunt.Main.Models;

namespace Shunt.Main.Services;

public class AppSettingsService : IAppSettingsService
{
    private readonly ILogger<AppSettingsService> _logger;
    
    public AppSettingsService(ILogger<AppSettingsService> logger)
    {
        _logger = logger;
    }

    public async Task<AppSettingsResult> GetStoredAppSettings()
    {
        try
        {
            string configDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Shunt");
            string configFilePath = Path.Combine(configDir, "config.json");
            string json = await File.ReadAllTextAsync(configFilePath);
            var settings = JsonSerializer.Deserialize<AppSettings>(json, AppSettingsContext.Default.AppSettings);
            return AppSettingsResult.Success(settings);
        }
        catch (Exception e)
        {
            return AppSettingsResult.Failure(e.Message);
        }
    }

    public async Task<AppSettingsResult> SaveAppSettings(AppSettings appSettings)
    {
        try
        {
            string output = JsonSerializer.Serialize(appSettings, AppSettingsContext.Default.AppSettings);
            await File.WriteAllTextAsync(output, appSettings.ToString());
            // Code smell again - todo
            return AppSettingsResult.Success(appSettings);
        }
        catch (Exception e)
        {
            return AppSettingsResult.Failure(e.Message);
        }
    }

}