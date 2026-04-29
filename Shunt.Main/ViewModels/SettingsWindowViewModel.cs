using System.Globalization;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Shunt.Main.Interfaces;
using Shunt.Main.Models;
using Shunt.Main.Utilities;

namespace Shunt.Main.ViewModels;

public partial class SettingsWindowViewModel : ViewModelBase
{
    private readonly ILogger<SettingsWindowViewModel> _logger;
    private readonly IAppSettingsService _appSettingsService;
    private readonly CachedAppSettings _cachedAppSettings;
    private readonly ISecretStoreService _secretStoreService;

    private string _serverPortText = null!;
    private string _contextTokenLengthText = null!;
    private string _defaultServiceIp = null!;
    private string _defaultModel = null!;
    private string _apiKey = null!;
    private string _errorMessage = null!;
    private bool _enableAutoLoad;

    public string ServerPortText
    {
        get => _serverPortText;
        set => SetProperty(ref _serverPortText, value);
    }

    public string ContextTokenLengthText
    {
        get => _contextTokenLengthText;
        set => SetProperty(ref _contextTokenLengthText, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public string DefaultServiceIp
    {
        get => _defaultServiceIp;
        set => SetProperty(ref _defaultServiceIp, value);
    }

    public string DefaultModel
    {
        get => _defaultModel;
        set => SetProperty(ref _defaultModel, value);
    }

    public string ApiKey
    {
        get => _apiKey;
        set => SetProperty(ref _apiKey, value);
    }

    public bool EnableAutoLoad
    {
        get => _enableAutoLoad;
        set => SetProperty(ref _enableAutoLoad, value);
    }

    public SettingsWindowViewModel(ILogger<SettingsWindowViewModel> logger, IAppSettingsService appSettingsService,
        CachedAppSettings cachedAppSettings, ISecretStoreService secretStoreService)
    {
        _logger = logger;
        _appSettingsService = appSettingsService;
        _cachedAppSettings = cachedAppSettings;
        _secretStoreService = secretStoreService;

        ServerPortText = _cachedAppSettings.ServerPort.ToString(CultureInfo.InvariantCulture);
        ContextTokenLengthText = _cachedAppSettings.ContextTokenLength.ToString(CultureInfo.InvariantCulture);
        DefaultServiceIp = _cachedAppSettings.ServerIp;
        DefaultModel = _cachedAppSettings.DefaultModel;
        ApiKey = string.Empty;
        EnableAutoLoad = _cachedAppSettings.EnableAutoLoadUnload;
        ErrorMessage = string.Empty;

        _ = LoadApiKeyAsync();
    }

    [RelayCommand]
    public async Task SaveSettings()
    {
        ErrorMessage = string.Empty;

        if (!int.TryParse(ServerPortText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var port) ||
            port < 1 || port > 65535)
        {
            ErrorMessage = "Server port must be a number between 1 and 65535.";
            return;
        }

        if (!int.TryParse(ContextTokenLengthText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var tokenLength) ||
            tokenLength <= 0)
        {
            ErrorMessage = "Context token length must be a positive number.";
            return;
        }

        if (!ServerAddressHelper.TryNormaliseServerHost(DefaultServiceIp, out var normalisedServerHost, out var serverAddressError))
        {
            ErrorMessage = serverAddressError;
            return;
        }

        if (string.IsNullOrWhiteSpace(DefaultModel))
        {
            ErrorMessage = "Default model is required.";
            return;
        }

        var settings = new AppSettings
        {
            ServerPort = port,
            ServerIp = normalisedServerHost,
            DefaultModel = DefaultModel.Trim(),
            ContextTokenLength = tokenLength,
            EnableAutoLoadUnload = EnableAutoLoad
        };

        var result = await _appSettingsService.SaveAppSettings(settings);
        if (!result.IsSuccess)
        {
            ErrorMessage = result.ErrorMessage;
            _logger.LogError("Failed to save settings: {ErrorMessage}", result.ErrorMessage);
            return;
        }


        var apiKeyResult = await _secretStoreService.SaveApiKey(ApiKey.Trim());
        if (!apiKeyResult.IsSuccess)
        {
            ErrorMessage = apiKeyResult.ErrorMessage;
            _logger.LogError("Failed to save API key: {ErrorMessage}", apiKeyResult.ErrorMessage);
            return;
        }


        ServerPortText = port.ToString(CultureInfo.InvariantCulture);
        ContextTokenLengthText = tokenLength.ToString(CultureInfo.InvariantCulture);
        DefaultServiceIp = normalisedServerHost;
        DefaultModel = settings.DefaultModel;
        ApiKey = ApiKey.Trim();
        _cachedAppSettings.ServerPort = port;
        _cachedAppSettings.ServerIp = normalisedServerHost;
        _cachedAppSettings.DefaultModel = settings.DefaultModel;
        _cachedAppSettings.ContextTokenLength = tokenLength;
        _cachedAppSettings.EnableAutoLoadUnload = EnableAutoLoad;
        _logger.LogInformation("Settings saved successfully.");
    }

    private async Task LoadApiKeyAsync()
    {
        var result = await _secretStoreService.GetApiKey();
        if (result.IsSuccess)
        {
            ApiKey = result.Key;
            return;
        }

        _logger.LogInformation("API key was not loaded into settings: {ErrorMessage}", result.ErrorMessage);
    }
}