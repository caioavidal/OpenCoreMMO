using NeoServer.Game.Contracts;
using NeoServer.Game.Common.Location;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Contracts.Items.Types;

namespace NeoServer.Server.Commands.Movement
{
    public class MapToContainerMovementOperation
    {
        public static void Execute(IPlayer player, Game game, IMap map, ItemThrowPacket itemThrow)
        {
            WalkToMechanism.DoOperation(player, () => MapToContainer(player, map, itemThrow), itemThrow.FromLocation, game);
        }

        private static void MapToContainer(IPlayer player, IMap map, ItemThrowPacket itemThrow)
        {
            if (map[itemThrow.FromLocation] is not IDynamicTile fromTile) return;

            if (fromTile.TopItemOnStack is not IPickupable item) return;

            if (!itemThrow.FromLocation.IsNextTo(player.Location))
            {
                player.WalkTo(itemThrow.FromLocation);
            }

            var container = player.Containers[itemThrow.ToLocation.ContainerId];

            player.MoveThing(fromTile, container, item, itemThrow.Count, 0, (byte)itemThrow.ToLocation.ContainerSlot);
        }

        public static bool IsApplicable(ItemThrowPacket itemThrowPacket) =>
              itemThrowPacket.FromLocation.Type == LocationType.Ground
              && itemThrowPacket.ToLocation.Type == LocationType.Container;

    }
}
