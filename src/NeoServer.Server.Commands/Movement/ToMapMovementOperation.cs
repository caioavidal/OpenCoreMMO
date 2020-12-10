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
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Commands.Movement
{
    public class ToMapMovementOperation
    {
        public static void Execute(IPlayer player, Game game, IMap map, ItemThrowPacket itemThrow)
        {
            if (map[itemThrow.ToLocation] is not IDynamicTile toTile) return;
            //todo check if tile reached max stack count
            //todo check max throw distance

            FromGround(player, game, map, itemThrow);
            FromInventory(player, map, itemThrow);
            FromContainer(player, map, itemThrow);
        }

        private static Action<ICreature> callBack;

        private static void FromGround(IPlayer player, Game game, IMap map, ItemThrowPacket itemThrow, bool secondChance = false)
        {
            if (secondChance)
            {
                player.OnCompleteWalking -= callBack.Invoke;
            }

            if (itemThrow.FromLocation.Type != LocationType.Ground) return;

            if (map[itemThrow.FromLocation] is not IDynamicTile fromTile) return;

            if (fromTile.TopItemOnStack is not IMoveableThing thing) return;

            if (!itemThrow.FromLocation.IsNextTo(player.Location))
            {
                if (secondChance) return;

                callBack = (creature) => FromGround(player, game, map, itemThrow, true);

                player.WalkTo(itemThrow.FromLocation, callBack);
                return;
            }

            var delay = 0;

            if (secondChance)
            {
                delay = player.StepDelayMilliseconds;
            }

            game.Scheduler.AddEvent(new SchedulerEvent(delay, () => map.TryMoveThing(thing, itemThrow.ToLocation, itemThrow.Count)));
        }
        private static void FromInventory(IPlayer player, IMap map, ItemThrowPacket itemThrow)
        {
            if (itemThrow.FromLocation.Type is not LocationType.Slot) return;
            if (map[itemThrow.ToLocation] is not IDynamicTile toTile) return;
            if (player.Inventory[itemThrow.FromLocation.Slot] is not IPickupable item) return;

            if (player.Inventory.RemoveItemFromSlot(itemThrow.FromLocation.Slot, itemThrow.Count, out var removedItem) is false) return;

            map.AddItem(removedItem, toTile);
        }
        private static void FromContainer(IPlayer player, IMap map, ItemThrowPacket itemThrow)
        {
            if (itemThrow.FromLocation.Type is not LocationType.Container) return;
            if (map[itemThrow.ToLocation] is not IDynamicTile toTile) return;

            var container = player.Containers[itemThrow.FromLocation.ContainerId];
            if (container[itemThrow.FromLocation.ContainerSlot] is not IPickupable item) return;

            var removedItem = container.RemoveItem((byte)itemThrow.FromLocation.ContainerSlot, itemThrow.Count);
            map.AddItem(removedItem, toTile);
        }

        public static bool IsApplicable(ItemThrowPacket itemThrowPacket) => itemThrowPacket.ToLocation.Type == LocationType.Ground;
    }
}
