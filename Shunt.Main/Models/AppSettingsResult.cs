using System;

namespace Shunt.Main.Models;

/// <summary>
/// A specialized, allocation-free result for API Key operations.
/// Perfectly compatible with Native AOT.
/// </summary>
public readonly struct AppSettingsResult
{
    private readonly AppSettings? _appSettings;
    private readonly string? _error;
    private readonly bool _isSuccess;

    private AppSettingsResult(AppSettings? appSettings, string? error, bool isSuccess)
    {
        _appSettings = appSettings;
        _error = error;
        _isSuccess = isSuccess;
    }

    public bool IsSuccess => _isSuccess;
    public bool IsFailure => !_isSuccess;

    // Static factories for clean syntax
    public static AppSettingsResult Success(AppSettings appSettings) => new(appSettings, null, true);
    public static AppSettingsResult Failure(string errorMessage) => new(null, errorMessage, false);

    // Direct access properties
    public AppSettings? AppSettings => _isSuccess 
        ? _appSettings
        : throw new InvalidOperationException("Cannot access appSettings on a failed result.");

    public string ErrorMessage => !_isSuccess 
        ? _error ?? "Unknown Error" 
        : throw new InvalidOperationException("Cannot access ErrorMessage on a successful result.");

    // Optional: A simple helper for quick branching
    public bool TryGetSettings(out AppSettings? settings, out string error)
    {
        settings = _appSettings;
        error = _error ?? string.Empty;
        return _isSuccess;
    }
}