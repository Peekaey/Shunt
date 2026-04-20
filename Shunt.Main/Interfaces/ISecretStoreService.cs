using System.Threading.Tasks;
using Shunt.Main.Models;

namespace Shunt.Main.Interfaces;

public interface ISecretStoreService
{
    Task<SecretStoreResult> SaveApiKey(string apiKey);
    Task<SecretStoreResult> GetApiKey();
}