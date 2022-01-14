using System.Threading.Tasks;

namespace NeoServer.Data.Interfaces;

public interface IPlayerRepository
{
    Task UpdateAllToOffline();
}