using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Items.Items
{
    public interface IUseableOnItem
    {
        void UseOn(IPlayer player, IMap map, IThing itemId);
    }
}