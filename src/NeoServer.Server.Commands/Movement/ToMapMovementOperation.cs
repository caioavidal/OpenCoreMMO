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
            if (!(map[itemThrow.ToLocation] is IDynamicTile toTile)) return;
            
            if (itemThrow.FromLocation.Type == LocationType.Ground)
            {
                var fromTile = map[itemThrow.FromLocation];
                if (fromTile is null) return;

                if (!(fromTile is IDynamicTile tile)) return;

                var thing = tile.TopItemOnStack as IThing;

                if (thing is null || !(thing is IMoveableThing)) return;

                if (!itemThrow.FromLocation.IsNextTo(player.Location))
                {
                    player.WalkTo(itemThrow.FromLocation);
                }

                //if(thing is IItem item) map.TryMoveItem(item, toTile.Location);
                //if (thing is ICreature creature) map.TryMoveCreature(creature, toTile.Location);

            }
        }

        public static bool IsApplicable(ItemThrowPacket itemThrowPacket) => itemThrowPacket.ToLocation.Type == LocationType.Ground;
    }
}
