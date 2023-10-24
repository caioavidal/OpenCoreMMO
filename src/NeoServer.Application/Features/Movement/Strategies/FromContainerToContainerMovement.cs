using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Location;
using NeoServer.Networking.Packets.Incoming;

namespace NeoServer.Application.Features.Movement.Strategies;

public class FromContainerToContainerMovement : IItemMovement
{
    public void Handle(IPlayer player, ItemThrowPacket itemThrowPacket)
    {
        if (itemThrowPacket.FromLocation.Type != LocationType.Container
            || itemThrowPacket.ToLocation.Type != LocationType.Container)
            return;

        var fromContainerId = itemThrowPacket.FromLocation.ContainerId;
        var toContainerId = itemThrowPacket.ToLocation.ContainerId;
        var itemIndex = itemThrowPacket.FromLocation.ContainerSlot;

        var sameIds = fromContainerId == toContainerId;
        var sameContainers = player.Containers[fromContainerId][itemIndex] == player.Containers[toContainerId];

        var tryingMoveContainerToItself = !sameIds && sameContainers;

        if (tryingMoveContainerToItself)
            //this is impossible error
            return;

        player.Containers.MoveItemBetweenContainers(itemThrowPacket.FromLocation, itemThrowPacket.ToLocation,
            itemThrowPacket.Count);
    }

    public string MovementKey => $"{LocationType.Container.ToString()}-{LocationType.Container.ToString()}";
}