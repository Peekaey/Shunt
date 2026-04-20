using System;

namespace Shunt.Main.Models;

/// <summary>
/// A specialized, allocation-free result for API Key operations.
/// Perfectly compatible with Native AOT.
/// </summary>
public readonly struct SecretStoreResult
{
    private readonly string? _key;
    private readonly string? _error;
    private readonly bool _isSuccess;

    private SecretStoreResult(string? key, string? error, bool isSuccess)
    {
        _key = key;
        _error = error;
        _isSuccess = isSuccess;
    }

    public bool IsSuccess => _isSuccess;
    public bool IsFailure => !_isSuccess;

    // Static factories for clean syntax
    public static SecretStoreResult Success(string key) => new(key, null, true);
    public static SecretStoreResult Failure(string errorMessage) => new(null, errorMessage, false);

    // Direct access properties
    public string Key => _isSuccess 
        ? _key! 
        : throw new InvalidOperationException("Cannot access Key on a failed result.");

    public string ErrorMessage => !_isSuccess 
        ? _error ?? "Unknown Error" 
        : throw new InvalidOperationException("Cannot access ErrorMessage on a successful result.");

    // Optional: A simple helper for quick branching
    public bool TryGetKey(out string key, out string error)
    {
        key = _key ?? string.Empty;
        error = _error ?? string.Empty;
        return _isSuccess;
    }
}