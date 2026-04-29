using System.Threading.Tasks;
using Shunt.Main.Models;

namespace Shunt.Main.Interfaces;

public interface IApiManager
{
    Task<ModelListResponse?> GetEndpointModels();
    Task<ServiceResult> LoadModel();
    Task<ServiceResult> UnloadModel();
}