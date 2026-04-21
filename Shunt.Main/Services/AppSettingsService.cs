using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shunt.Main.Interfaces;
using Shunt.Main.Models;
using Shunt.Main.Models.Results;

namespace Shunt.Main.Services;

public class AppSettingsService : IAppSettingsService
{
    private readonly ILogger<AppSettingsService> _logger;
    private readonly string _configFilePath;

    public AppSettingsService(ILogger<AppSettingsService> logger)
    {
        _logger = logger;
        string configDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Shunt");
        _configFilePath = Path.Combine(configDir, "config.json");
        
        if (!Directory.Exists(configDir))
        {
            Directory.CreateDirectory(configDir);
        }
    }

    public async Task<AppSettingsResult> GetStoredAppSettings()
    {
        try
        {
            if (!File.Exists(_configFilePath))
            {
                return AppSettingsResult.Failure("Config file does not exist.");
            }

            string json = await File.ReadAllTextAsync(_configFilePath);
            var settings = JsonSerializer.Deserialize(json, AppSettingsContext.Default.AppSettings);
            
            return settings != null 
                ? AppSettingsResult.Success(settings) 
                : AppSettingsResult.Failure("Failed to deserialize settings.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error reading app settings");
            return AppSettingsResult.Failure(e.Message);
        }
    }

    public async Task<ServiceResult> SaveAppSettings(AppSettings appSettings)
    {
        try
        {
            string json = JsonSerializer.Serialize(appSettings, AppSettingsContext.Default.AppSettings);
            await File.WriteAllTextAsync(_configFilePath, json);
            return ServiceResult.Success();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error saving app settings");
            return ServiceResult.Failure(e.Message);
        }
    }
}