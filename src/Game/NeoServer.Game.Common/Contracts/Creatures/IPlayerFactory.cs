using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Creatures
{
    public interface IPlayerFactory
    {
        IPlayer Create(IPlayer player);
    }
}