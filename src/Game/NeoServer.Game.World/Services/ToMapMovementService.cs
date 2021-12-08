using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Creatures.Structs;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Texts;
using NeoServer.Game.World.Map.Tiles;

namespace NeoServer.Game.World.Services
{
    public class ToMapMovementService : IToMapMovementService
    {
        private readonly IMap map;
        private readonly IMapService mapService;

        public ToMapMovementService(IMap map, IMapService mapService)
        {
            this.map = map;
            this.mapService = mapService;
        }

        public void Move(IPlayer player, MovementParams itemThrow)
        {
            var finalTile = mapService.GetFinalTile(itemThrow.ToLocation);

            if (finalTile is not IDynamicTile)
            {
                OperationFailService.Display(player.CreatureId, TextConstants.NOT_ENOUGH_ROOM);
                return;
            }

            FromGround(player, itemThrow);
            FromInventory(player, itemThrow);
            FromContainer(player, itemThrow);
        }

        private void FromGround(IPlayer player, MovementParams movementParams)
        {
            if (movementParams.FromLocation.Type != LocationType.Ground) return;

            if (map[movementParams.FromLocation] is not Tile fromTile) return;
            if (map[movementParams.ToLocation] is not Tile toTile) return;

            if (fromTile.TopItemOnStack is not { } item) return;

            var finalTile = (Tile)mapService.GetFinalTile(toTile.Location);

            player.MoveItem(fromTile, finalTile, item, movementParams.Amount, 0, 0);
        }

        private void FromInventory(IPlayer player, MovementParams movementParams)
        {
            if (movementParams.FromLocation.Type is not LocationType.Slot) return;
            if (map[movementParams.ToLocation] is not IDynamicTile toTile) return;
            if (player.Inventory[movementParams.FromLocation.Slot] is not IPickupable item) return;

            var finalTile = (Tile)mapService.GetFinalTile(toTile.Location);

            player.MoveItem(player.Inventory, finalTile, item, movementParams.Amount,
                (byte)movementParams.FromLocation.Slot, 0);
        }

        private void FromContainer(IPlayer player, MovementParams itemThrow)
        {
            if (itemThrow.FromLocation.Type is not LocationType.Container) return;
            if (map[itemThrow.ToLocation] is not IDynamicTile toTile) return;

            var container = player.Containers[itemThrow.FromLocation.ContainerId];
            if (container[itemThrow.FromLocation.ContainerSlot] is not IPickupable item) return;

            var finalTile = (Tile)mapService.GetFinalTile(toTile.Location);

            player.MoveItem(container, finalTile, item, itemThrow.Amount, (byte)itemThrow.FromLocation.ContainerSlot, 0);
        }
    }
}