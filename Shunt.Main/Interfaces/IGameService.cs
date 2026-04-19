using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shunt.Main.Interfaces;

public interface IGameService
{
    Task<HashSet<string>> CheckForRunningSteamGame();
}