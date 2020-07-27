
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Enums.Players;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Commands.Movement
{
    public sealed class MapToInventoryMovementOperation
    {
        public static void Execute(IPlayer player, IMap map, ItemThrowPacket itemThrow)
        {
            var tile = map[itemThrow.FromLocation] as IWalkableTile;
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

            var result = player.Inventory.TryAddItemToSlot(itemThrow.ToLocation.Slot, item as IPickupable, amount);

            if (!result.Success)
            {
                return;
            }

            map.RemoveThing(ref thing, tile, amount);
            
            if(result.Value != null)
            {
                var returnedThing = result.Value as IMoveableThing;
                map.AddItem(ref returnedThing, tile, amount);
            }
        }

        public static bool IsApplicable(ItemThrowPacket itemThrowPacket) =>
             itemThrowPacket.FromLocation.Type == LocationType.Ground
             && itemThrowPacket.ToLocation.Type == LocationType.Slot;
    }
}
