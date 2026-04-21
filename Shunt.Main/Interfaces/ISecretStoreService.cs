using System.Threading.Tasks;
using Shunt.Main.Models;
using Shunt.Main.Models.Results;

namespace Shunt.Main.Interfaces;

public interface ISecretStoreService
{
    Task<ServiceResult> SaveApiKey(string apiKey);
    Task<SecretStoreResult> GetApiKey();
}