﻿using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Commands.Movements
{
    public class ToMapMovementOperation
    {
        public static void Execute(IPlayer player, IGameServer game, IMap map, ItemThrowPacket itemThrow)
        {
            if (map[itemThrow.ToLocation] is not IDynamicTile toTile) return;

            FromGround(player, map, itemThrow);
            FromInventory(player, map, itemThrow);
            FromContainer(player, map, itemThrow);
        }

        private static void FromGround(IPlayer player, IMap map, ItemThrowPacket itemThrow, bool secondChance = false)
        {
            if (itemThrow.FromLocation.Type != LocationType.Ground) return;

            if (map[itemThrow.FromLocation] is not IDynamicTile fromTile) return;

            if (fromTile.TopItemOnStack is not IItem item) return;

            player.MoveItem(fromTile, map[itemThrow.ToLocation], item, itemThrow.Count, 0, 0);
        }

        private static void FromInventory(IPlayer player, IMap map, ItemThrowPacket itemThrow)
        {
            if (itemThrow.FromLocation.Type is not LocationType.Slot) return;
            if (map[itemThrow.ToLocation] is not IDynamicTile toTile) return;
            if (player.Inventory[itemThrow.FromLocation.Slot] is not IPickupable item) return;

            player.MoveItem(player.Inventory, toTile, item, itemThrow.Count, (byte) itemThrow.FromLocation.Slot, 0);
        }

        private static void FromContainer(IPlayer player, IMap map, ItemThrowPacket itemThrow)
        {
            if (itemThrow.FromLocation.Type is not LocationType.Container) return;
            if (map[itemThrow.ToLocation] is not IDynamicTile toTile) return;

            var container = player.Containers[itemThrow.FromLocation.ContainerId];
            if (container[itemThrow.FromLocation.ContainerSlot] is not IPickupable item) return;

            player.MoveItem(container, toTile, item, itemThrow.Count, (byte) itemThrow.FromLocation.ContainerSlot, 0);
        }

        public static bool IsApplicable(ItemThrowPacket itemThrowPacket)
        {
            return itemThrowPacket.ToLocation.Type == LocationType.Ground;
        }
    }
}