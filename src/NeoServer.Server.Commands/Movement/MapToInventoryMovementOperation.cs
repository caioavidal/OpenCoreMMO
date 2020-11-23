
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
        public static void Execute(IPlayer player, IMap map, ItemThrowPacket itemThrow)
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

            var clonedItem = thing is ICumulativeItem cumulative ? cumulative.Clone(amount) : thing;

            var result = player.Inventory.TryAddItemToSlot(itemThrow.ToLocation.Slot, clonedItem as IPickupable);

            if (!result.Success)
            {
                return;
            }

            var amountToRemove = amount;

            var thingToRemove = thing as IThing;

            if (result.Value?.ClientId == item.ClientId && thing is ICumulativeItem)
            {
                amountToRemove = (byte)(amount - (result.Value as ICumulativeItem).Amount);
                map.RemoveThing(ref thingToRemove, tile, amountToRemove);

                return;
            }

            map.RemoveThing(ref thingToRemove, tile, amountToRemove);

            if (result.Value != null)
            {
                var returnedThing = result.Value as IThing;
                map.AddItem(ref returnedThing, tile, amount);
            }
        }

        public static bool IsApplicable(ItemThrowPacket itemThrowPacket) =>
             itemThrowPacket.FromLocation.Type == LocationType.Ground
             && itemThrowPacket.ToLocation.Type == LocationType.Slot;
    }
}
