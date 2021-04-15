using NeoServer.Server.Model.Players;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Loaders.Interfaces
{
    public interface IPlayerLoader
    {
        IPlayer Load(PlayerModel player);
        bool IsApplicable(PlayerModel player);
    }
}
