using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shunt.Main.Interfaces;

namespace Shunt.Main.Services;

public class GameService :IGameService
{
    private readonly ILogger<GameService> _logger;
    
    public GameService(ILogger<GameService> logger)
    {
        _logger = logger;
    }

    // Checks system processes for applications that have an environment variable of
    // STEAM_COMPAT_APP_ID or SteamAppId as these are injected by steam during launch and indicated it was launched by steam.
    // Works for native games like Terraria and non-native games like Deadlock/Red Dead Redemption 2
    public async Task<HashSet<string>> CheckForRunningSteamGame()
    {
        return await Task.Run(() =>
        {
            var runningAppIds = new HashSet<string>();

            var directories = Directory.GetDirectories("/proc");

            foreach (var directory in directories)
            {
                string processId = Path.GetFileName(directory);

                if (!int.TryParse(processId, out _))
                {
                    continue;
                }

                string environmentPath = Path.Combine(directory, "environ");

                try
                {
                    byte[] bytes = File.ReadAllBytes(environmentPath);
                    string environment = System.Text.Encoding.UTF8.GetString(bytes);

                    string[] vars = environment.Split('\0');

                    foreach (var v in vars)
                    {
                        if (v.StartsWith("STEAM_COMPAT_APP_ID=") || v.StartsWith("SteamAppId="))
                        {
                            string idPart = v.Split('=')[1];
                            if (int.TryParse(idPart, out int appId))
                            {
                                Console.WriteLine($"Found running Steam game with App ID: {appId}");
                                runningAppIds.Add(idPart);
                            }
                        }
                    }
                }
                catch (UnauthorizedAccessException) { /* Not related to our process, swallow and skip */ }
                catch (IOException) { /* Process closed during read and is no longer relevant, swallow and skip */ }
                catch (Exception e)
                {
                    // Console.WriteLine(e);
                }
            }

            return runningAppIds;
        });
    }
}