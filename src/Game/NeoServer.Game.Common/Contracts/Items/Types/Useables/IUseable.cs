using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Common.Contracts.Items.Types.Useables
{
    public interface IUseable : IThing
    {
        void Use(IPlayer player);
    }
}