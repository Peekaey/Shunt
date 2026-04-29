using System.Threading.Tasks;
using Shunt.Main.Models;
using Shunt.Main.Models.Results;

namespace Shunt.Main.Interfaces;

public interface IAppSettingsService
{
    Task<AppSettingsResult> GetStoredAppSettings();
    Task<ServiceResult> SaveAppSettings(AppSettings appSettings);
    Task<AppSettingsResult>ClearAndSaveDefaultAppSettings(AppSettings appSettings);
}