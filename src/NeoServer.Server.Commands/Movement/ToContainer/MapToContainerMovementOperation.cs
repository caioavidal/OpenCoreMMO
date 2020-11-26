using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Common.Location;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Contracts.Items.Types;

namespace NeoServer.Server.Commands.Movement
{
    public class MapToContainerMovementOperation
    {

        public static void Execute(IPlayer player, IMap map, ItemThrowPacket itemThrow)
        {

            if (map[itemThrow.FromLocation] is not IDynamicTile fromTile) return;
            

            if (fromTile.TopItemOnStack is not IPickupable item) return;

            if (!itemThrow.FromLocation.IsNextTo(player.Location))
            {
                player.WalkTo(itemThrow.FromLocation);
            }

            var container = player.Containers[itemThrow.ToLocation.ContainerId];

            if (container is null) return;

            IItem itemToAdd = item;

            if(item is ICumulativeItem cumulative)
            {
                itemToAdd = cumulative.Clone(itemThrow.Count);
            }

            if (container.TryAddItem(itemToAdd).Success is false) return;

            map.RemoveThing(item, fromTile, itemThrow.Count);
        }
        public static bool IsApplicable(ItemThrowPacket itemThrowPacket) =>
              itemThrowPacket.FromLocation.Type == LocationType.Ground
              && itemThrowPacket.ToLocation.Type == LocationType.Container;

    }
}
