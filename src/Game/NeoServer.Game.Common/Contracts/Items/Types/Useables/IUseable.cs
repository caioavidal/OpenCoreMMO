using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Contracts.Items
{
    public interface IUseable : IThing
    {
        void Use(IPlayer player);
    }
}