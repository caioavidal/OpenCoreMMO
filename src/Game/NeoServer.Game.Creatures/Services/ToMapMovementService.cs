using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Creatures.Structs;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Texts;

namespace NeoServer.Game.Creatures.Services
{
    public class ToMapMovementService : IToMapMovementService
    {
        private readonly IMap map;

        public ToMapMovementService(IMap map)
        {
            this.map = map;
        }
        
        public void Move(IPlayer player, MovementParams itemThrow)
        {
            var finalTile = map.GetFinalTile(map[itemThrow.ToLocation]);

            if (finalTile is not IDynamicTile)
            {
                OperationFailService.Display(player.CreatureId, TextConstants.NOT_ENOUGH_ROOM);
                return;
            };
            
            FromGround(player, map, itemThrow);
            FromInventory(player, map, itemThrow);
            FromContainer(player, map, itemThrow);
        }

      

        private void FromGround(IPlayer player, IMap map, MovementParams movementParams)
        {
            if (movementParams.FromLocation.Type != LocationType.Ground) return;

            if (map[movementParams.FromLocation] is not IDynamicTile fromTile) return;

            if (fromTile.TopItemOnStack is not IItem item) return;

            player.MoveItem(fromTile, map[movementParams.ToLocation], item, movementParams.Amount, 0, 0);
        }

        private static void FromInventory(IPlayer player, IMap map, MovementParams movementParams)
        {
            if (movementParams.FromLocation.Type is not LocationType.Slot) return;
            if (map[movementParams.ToLocation] is not IDynamicTile toTile) return;
            if (player.Inventory[movementParams.FromLocation.Slot] is not IPickupable item) return;

            player.MoveItem(player.Inventory, toTile, item, movementParams.Amount, (byte) movementParams.FromLocation.Slot, 0);
        }

        private static void FromContainer(IPlayer player, IMap map, MovementParams itemThrow)
        {
            if (itemThrow.FromLocation.Type is not LocationType.Container) return;
            if (map[itemThrow.ToLocation] is not IDynamicTile toTile) return;

            var container = player.Containers[itemThrow.FromLocation.ContainerId];
            if (container[itemThrow.FromLocation.ContainerSlot] is not IPickupable item) return;

            player.MoveItem(container, toTile, item, itemThrow.Amount, (byte) itemThrow.FromLocation.ContainerSlot, 0);
        }
    }
}