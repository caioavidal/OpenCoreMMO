using NeoServer.Game.Common;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Items.Items;
using NeoServer.Server.Items;

namespace NeoServer.Game.Items.Factories
{
    public class LiquidPoolFactory: ILiquidPoolFactory
    {
        public event CreateItem OnItemCreated;

        public ILiquid Create(Location location, LiquidColor color)
        {
            if (!ItemTypeData.InMemory.TryGetValue(2016, out var itemType)) return null;

            if (itemType.Group == ItemGroup.ITEM_GROUP_DEPRECATED)
            {
                return null;
            }

            var item = new LiquidPoolItem(itemType, location, color);
            OnItemCreated?.Invoke(item);
            return item;

        }
        public ILiquid CreateDamageLiquidPool(Location location, LiquidColor color)
        {
            if (!ItemTypeData.InMemory.TryGetValue(2019, out var itemType)) return null;

            if (itemType.Group == ItemGroup.ITEM_GROUP_DEPRECATED)
            {
                return null;
            }

            var item = new LiquidPoolItem(itemType, location, color);
            OnItemCreated?.Invoke(item);
            return item;
        }
    }
}
