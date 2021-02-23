using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Common.Contracts.Services
{
    public interface IDealTransaction
    {
        bool Buy(IPlayer buyer, IShopperNpc seller, IItemType itemType, byte amount);
    }
}
