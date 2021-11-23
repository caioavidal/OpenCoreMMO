using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;

namespace NeoServer.Game.Items.Events
{
    public class ItemTransformedEventHandler: IGameEventHandler
    {
        private readonly IMap map;
        private readonly IItemFactory itemFactory;

        public ItemTransformedEventHandler(IMap map, IItemFactory itemFactory)
        {
            this.map = map;
            this.itemFactory = itemFactory;
        }

        public void Execute(IPlayer player, IItem fromItem, ushort toItem)
        {
            var createdItem = itemFactory.Create(toItem, fromItem.Location, null);

            ReplaceItemOnGround(fromItem, createdItem);
            ReplaceItemOnContainer(player, fromItem, createdItem);
        }

        private static void ReplaceItemOnContainer(IPlayer player, IItem fromItem, IItem createdItem)
        {
            if (fromItem.Location.Type != LocationType.Container) return;

            var container = player.Containers[fromItem.Location.ContainerId] ?? player.Inventory?.BackpackSlot;

            var result = container is not null
                ? container.AddItem(createdItem)
                : new Result<OperationResult<IItem>>(InvalidOperation.NotPossible);

            if (!result.IsSuccess) player.Tile.AddItem(createdItem);
        }

        private void ReplaceItemOnGround(IItem fromItem, IItem createdItem)
        {
            if (fromItem.Location.Type != LocationType.Ground) return;
            if (map[fromItem.Location] is not IDynamicTile tile) return;
            
            tile.RemoveItem(fromItem, 1, 0, out var removedThing);
            tile.AddItem(createdItem);
        }
    }
}