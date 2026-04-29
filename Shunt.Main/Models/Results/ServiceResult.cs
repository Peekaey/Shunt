using System;

namespace Shunt.Main.Models;

public readonly struct ServiceResult
{
    private readonly string? _error;
    private readonly bool _isSuccess;

    private ServiceResult(string? error, bool isSuccess)
    {
        _error = error;
        _isSuccess = isSuccess;
    }
    
    public bool IsSuccess => _isSuccess;
    public bool IsError => _error != null;
    
    public static ServiceResult Success() => new (null, true);
    public static ServiceResult Failure(string errorMessage) => new (errorMessage, false);
    
    public string ErrorMessage => !_isSuccess 
        ? _error ?? "Unknown Error" 
        : throw new InvalidOperationException("Cannot access ErrorMessage on a successful result.");
    
    
}
