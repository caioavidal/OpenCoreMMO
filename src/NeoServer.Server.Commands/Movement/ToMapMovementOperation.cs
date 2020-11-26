using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoServer.Game.Contracts.Items.Types;

namespace NeoServer.Server.Commands.Movement
{
    public class ToMapMovementOperation
    {
        public static void Execute(IPlayer player, IMap map, ItemThrowPacket itemThrow)
        {
            if (map[itemThrow.ToLocation] is not IDynamicTile toTile) return;

            FromGround(player, map, itemThrow);
            FromInventory(player, map, itemThrow);
        }

        private static void FromGround(IPlayer player, IMap map, ItemThrowPacket itemThrow)
        {
            if (itemThrow.FromLocation.Type == LocationType.Ground)
            {
                if (map[itemThrow.FromLocation] is not IDynamicTile fromTile) return;

                if (fromTile.TopItemOnStack is not IMoveableThing thing) return;

                if (!itemThrow.FromLocation.IsNextTo(player.Location))
                {
                    player.WalkTo(itemThrow.FromLocation);
                }

                map.TryMoveThing(thing, itemThrow.ToLocation, itemThrow.Count);
            }
        }
        private static void FromInventory(IPlayer player, IMap map, ItemThrowPacket itemThrow)
        {
            if (itemThrow.FromLocation.Type is not LocationType.Slot) return;
            if (map[itemThrow.ToLocation] is not IDynamicTile toTile) return;
            if (player.Inventory[itemThrow.FromLocation.Slot] is not IPickupable item) return;

            //todo check if tile reached max stack count
            if (player.Inventory.RemoveItemFromSlot(itemThrow.FromLocation.Slot, itemThrow.Count, out var removedItem) is false) return;

            map.AddItem(removedItem, toTile);
        }

        public static bool IsApplicable(ItemThrowPacket itemThrowPacket) => itemThrowPacket.ToLocation.Type == LocationType.Ground;
    }
}
