using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Loaders.Interfaces
{
    public interface IPlayerLoader
    {
        IPlayer Load(PlayerModel player);
        bool IsApplicable(PlayerModel player);
    }
}
