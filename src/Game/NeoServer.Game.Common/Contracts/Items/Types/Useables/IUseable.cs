using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Contracts.Items
{
    public interface IUseable
    {
        void Use(IPlayer player);
    }
}