using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shunt.Main.Interfaces;
using Shunt.Main.Models;

namespace Shunt.Main.Services;

public class ApiManager 
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiManager> _logger;
    private readonly IAppSettingsService _settingsService;
    private readonly ISecretStoreService _secretStoreService;

    public ApiManager(
        HttpClient httpClient, 
        ILogger<ApiManager> logger, 
        IAppSettingsService settingsService, 
        ISecretStoreService secretStoreService)
    {       
        _httpClient = httpClient;
        _logger = logger;
        _settingsService = settingsService;
        _secretStoreService = secretStoreService;
    }

    public async Task<ModelListResponse?> GetEndpointModels()
    {
        var settingsResult = await _settingsService.GetStoredAppSettings();
        if (settingsResult.IsFailure)
        {
            _logger.LogWarning("Could not retrieve app settings: {Error}", settingsResult.ErrorMessage);
            return null;
        }

        var secretResult = await _secretStoreService.GetApiKey();
        if (secretResult.IsFailure)
        {
            _logger.LogWarning("Could not retrieve API key: {Error}", secretResult.ErrorMessage);
            return null;
        }

        var settings = settingsResult.AppSettings;
        string endpointUrl = $"http://{settings.ServerIp}:{settings.ServerPort}/api/v1/models";

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, endpointUrl);
            
            if (!string.IsNullOrEmpty(secretResult.Key))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", secretResult.Key);
            }
            
            using var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                
                var result = JsonSerializer.Deserialize(json, ApiModelsContext.Default.ModelListResponse);
                
                if (result != null)
                {
                    _logger.LogInformation("Successfully retrieved {Count} models", result.Models.Count);
                    return result;
                }
            }
            else
            {
                _logger.LogError("API request failed with status code: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching models from {Url}", endpointUrl);
        }

        return null;
    }

    public async Task<ServiceResult> LoadModel()
    {
        var settingsResult = await _settingsService.GetStoredAppSettings();
        if (settingsResult.IsFailure)
        {
            _logger.LogWarning("Could not retrieve app settings: {Error}", settingsResult.ErrorMessage);
        }
        
        var secretResult = await _secretStoreService.GetApiKey();
        if (secretResult.IsFailure)
        {
            _logger.LogWarning("Could not retrieve API key: {Error}", secretResult.ErrorMessage);
        }
        
        var settings = settingsResult.AppSettings;
        string endpointUrl = $"http://{settings.ServerIp}:{settings.ServerPort}/api/v1/models/load";
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, endpointUrl);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            if (!string.IsNullOrEmpty(secretResult.Key))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", secretResult.Key);
            }

            using var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResult.Failure($"API request failed with status code: {response.StatusCode}");
            }

            return ServiceResult.Success();
        }
        catch (Exception e)
        {
            return ServiceResult.Failure(e.Message);
        }
    }
}