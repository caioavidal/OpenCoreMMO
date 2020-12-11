
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Commands.Movement
{
    public sealed class MapToInventoryMovementOperation
    {
        public static void Execute(IPlayer player, Game game, IMap map, ItemThrowPacket itemThrow) => WalkToMechanism.DoOperation(player, () => FromMapToInventory(player, map, itemThrow), itemThrow.FromLocation, game);

        private static void FromMapToInventory(IPlayer player, IMap map, ItemThrowPacket itemThrow)
        {
            var tile = map[itemThrow.FromLocation] as IDynamicTile;
            var amount = itemThrow.Count;
            if ((tile.DownItems.TryPeek(out var item) && item.IsPickupable) == false)
            {
                return;
            }

            if (!player.CanSee(tile.Location))
            {
                return;
            }

            var thing = item as IMoveableThing;

            var clonedItem = thing is ICumulative cumulative ? cumulative.Clone(amount) : thing;

            var result = player.Inventory.TryAddItemToSlot(itemThrow.ToLocation.Slot, clonedItem as IPickupable);

            if (!result.Success)
            {
                return;
            }

            var amountToRemove = amount;

            var thingToRemove = thing as IThing;

            if (result.Value?.ClientId == item.ClientId && thing is ICumulative)
            {
                amountToRemove = (byte)(amount - (result.Value as ICumulative).Amount);
                map.RemoveThing(thingToRemove, tile, amountToRemove);

                return;
            }

            map.RemoveThing(thingToRemove, tile, amountToRemove);

            if (result.Value != null)
            {
                var returnedThing = result.Value as IThing;
                map.AddItem(returnedThing, tile);
            }
        }

        public static bool IsApplicable(ItemThrowPacket itemThrowPacket) =>
             itemThrowPacket.FromLocation.Type == LocationType.Ground
             && itemThrowPacket.ToLocation.Type == LocationType.Slot;
    }
}
