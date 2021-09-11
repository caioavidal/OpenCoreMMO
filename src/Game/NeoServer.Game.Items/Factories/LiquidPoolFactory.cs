using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.DataStore;
using NeoServer.Game.Items.Items;

namespace NeoServer.Game.Items.Factories
{
    public class LiquidPoolFactory : ILiquidPoolFactory
    {
        private readonly ItemTypeStore _itemTypeStore;

        public LiquidPoolFactory(ItemTypeStore itemTypeStore)
        {
            _itemTypeStore = itemTypeStore;
        }

        public event CreateItem OnItemCreated;

        public ILiquid Create(Location location, LiquidColor color)
        {
            if (!_itemTypeStore.TryGetValue(2016, out var itemType)) return null;

            if (itemType.Group == ItemGroup.ItemGroupDeprecated) return null;

            var item = new LiquidPool(itemType, location, color);
            OnItemCreated?.Invoke(item);
            return item;
        }

        public ILiquid CreateDamageLiquidPool(Location location, LiquidColor color)
        {
            if (!_itemTypeStore.TryGetValue(2019, out var itemType)) return null;

            if (itemType.Group == ItemGroup.ItemGroupDeprecated) return null;

            var item = new LiquidPool(itemType, location, color);
            OnItemCreated?.Invoke(item);
            return item;
        }
    }
}