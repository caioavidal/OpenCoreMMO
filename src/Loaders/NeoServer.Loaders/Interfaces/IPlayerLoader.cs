using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Model.Players;

namespace NeoServer.Loaders.Interfaces
{
    public interface IPlayerLoader
    {
        IPlayer Load(PlayerModel player);
        bool IsApplicable(PlayerModel player);
    }
}