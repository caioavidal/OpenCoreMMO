using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Enums.Location;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Commands.Movement
{
    public class MapToContainerMovementOperation
    {

        public static void Execute(IPlayer player, IMap map, IMoveableThing thing, ItemThrowPacket itemThrowPacket)
        {
            map.ThrowIfNull();
            player.ThrowIfNull();

            if (!IsApplicable(itemThrowPacket))
            {
                return;
            }

            var tile = map[itemThrowPacket.FromLocation];
            var containerId = itemThrowPacket.ToLocation.ContainerId;

            // map.RemoveThing(ref thing, tile);
            player.Containers.MoveItemBetweenContainers(itemThrowPacket.FromLocation, itemThrowPacket.ToLocation, itemThrowPacket.Count);

        }
        public static bool IsApplicable(ItemThrowPacket itemThrowPacket) =>
              itemThrowPacket.FromLocation.Type == LocationType.Ground
              && itemThrowPacket.ToLocation.Type == LocationType.Container;

    }
}
