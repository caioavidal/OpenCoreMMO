using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Creatures
{
    public interface IPlayerFactory
    {
        IPlayer Create(IPlayer player);
    }
}