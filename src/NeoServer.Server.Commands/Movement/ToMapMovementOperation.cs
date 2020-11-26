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

namespace NeoServer.Server.Commands.Movement
{
    public class ToMapMovementOperation
    {
        public static void Execute(IPlayer player, IMap map, ItemThrowPacket itemThrow)
        {
            if (map[itemThrow.ToLocation] is not IDynamicTile toTile) return;
            
            if (itemThrow.FromLocation.Type == LocationType.Ground)
            {
                if (map[itemThrow.FromLocation] is not IDynamicTile tile) return;

                if (tile.TopItemOnStack is not IMoveableThing thing) return;

                if (!itemThrow.FromLocation.IsNextTo(player.Location))
                {
                    player.WalkTo(itemThrow.FromLocation);
                }

                map.TryMoveThing(thing, itemThrow.ToLocation, itemThrow.Count);
            }
        }
        public static bool IsApplicable(ItemThrowPacket itemThrowPacket) => itemThrowPacket.ToLocation.Type == LocationType.Ground;
    }
}
