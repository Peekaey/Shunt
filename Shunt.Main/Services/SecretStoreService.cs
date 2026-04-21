using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;
using DBus.Services.Secrets;
using Microsoft.Extensions.Logging;
using Shunt.Main.Interfaces;
using Shunt.Main.Models;
using Shunt.Main.Models.Results;

namespace Shunt.Main.Services;

public class SecretStoreService : ISecretStoreService
{
    private readonly string KeyLabel = "SHUNT_API_KEY";
    private readonly Dictionary<string, string> _searchAttributes = new()
    {
        { "application", "Shunt" }
    };
    private readonly ILogger<SecretStoreService> _logger;
    
    public SecretStoreService(ILogger<SecretStoreService> logger)
    {
        _logger = logger;
    }
    
    public async Task<ServiceResult> SaveApiKey(string apiKey)
    {
        try
        {

            var connection = DBusConnection.Session;
            var secretService = await SecretService.ConnectAsync(EncryptionType.Dh);

            var collection = await secretService.GetDefaultCollectionAsync();
            if (collection == null)
            {
                return ServiceResult.Failure("Failed to access the default secret collection.");
            }

            var secretBytes = Encoding.UTF8.GetBytes(apiKey);

            await collection.CreateItemAsync(KeyLabel, _searchAttributes, secretBytes, "text/plain", replace: true);
            // Code smell - fix later
            return ServiceResult.Success();
        }
        catch (Exception e)
        {
            return ServiceResult.Failure(e.Message);
        }
    }

    public async Task<SecretStoreResult> GetApiKey()
    {
        try
        {
            var secretService = await SecretService.ConnectAsync(EncryptionType.Dh);
            var collection = await secretService.GetDefaultCollectionAsync();

            if (collection == null)
            {
                return SecretStoreResult.Failure("Failed to access the default secret collection.");
            }

            var items = await collection.SearchItemsAsync(_searchAttributes);

            if (items.Length == 0)
            {
                return SecretStoreResult.Failure("No API key found in the secret store.");
            }

            var secret = await items[0].GetSecretAsync();
            return SecretStoreResult.Success(Encoding.UTF8.GetString(secret));
        }
        catch (Exception e)
        {
            return SecretStoreResult.Failure(e.Message);
        }
    }
}