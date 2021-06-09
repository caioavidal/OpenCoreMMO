using NeoServer.Game.Contracts.Items;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface IShopItem
    {
        IItemType Item { get; }
        uint BuyPrice { get; }
        uint SellPrice { get; }
    }
}