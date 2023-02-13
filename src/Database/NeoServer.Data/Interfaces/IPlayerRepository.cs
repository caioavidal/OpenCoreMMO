using System.Threading.Tasks;
using NeoServer.Data.Model;

namespace NeoServer.Data.Interfaces;

public interface IPlayerRepository
{
    Task UpdateAllToOffline();
    Task Add(PlayerModel player);
}