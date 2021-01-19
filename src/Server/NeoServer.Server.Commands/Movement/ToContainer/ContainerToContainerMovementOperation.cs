using NeoServer.Game.Common.Location;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Commands.Movement
{
    public class ContainerToContainerMovementOperation
    {

        public static void Execute(IPlayer player, ItemThrowPacket itemThrowPacket)
        {
            if (itemThrowPacket.FromLocation.Type != LocationType.Container
                || itemThrowPacket.ToLocation.Type != LocationType.Container)
            {
                return;
            }

            var fromContainerId = itemThrowPacket.FromLocation.ContainerId;
            var toContainerId = itemThrowPacket.ToLocation.ContainerId;
            var itemIndex = itemThrowPacket.FromLocation.ContainerSlot;

            var sameIds = fromContainerId == toContainerId;
            var sameContainers = player.Containers[fromContainerId][itemIndex] == player.Containers[toContainerId];

            var tryingMoveContainerToItself = !sameIds && sameContainers;

            if (tryingMoveContainerToItself)
            {
                //this is impossible error
                return;
            }

            player.Containers.MoveItemBetweenContainers(itemThrowPacket.FromLocation, itemThrowPacket.ToLocation, itemThrowPacket.Count);

        }
        public static bool IsApplicable(ItemThrowPacket itemThrowPacket) =>
              itemThrowPacket.FromLocation.Type == LocationType.Container
              && itemThrowPacket.ToLocation.Type == LocationType.Container;

    }
}
