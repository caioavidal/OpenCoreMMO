using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Common.Contracts.Items.Types.Usable
{
    public interface IUsable : IThing
    {
        void Use(IPlayer player);
    }
}