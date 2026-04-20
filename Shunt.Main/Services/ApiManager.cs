using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace Shunt.Main.Services;

public class ApiManager 
{
    private readonly  HttpClient _httpClient;
    private readonly ILogger<ApiManager> _logger;
    public ApiManager(HttpClient httpClient, ILogger<ApiManager> logger)
    {       
        _httpClient = httpClient;
        _logger = logger;
        
    }
}