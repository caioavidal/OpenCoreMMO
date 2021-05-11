using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;

namespace NeoServer.Game.Creatures.Npcs
{
    public record ShopItem(IItemType Item, uint BuyPrice, uint SellPrice) : IShopItem
    {
    }
}
