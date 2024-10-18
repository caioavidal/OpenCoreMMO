using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Networking.Packets.Outgoing.Player;

namespace NeoServer.Modules.ItemManagement.ContainerManagement.OpenContainer;

public class PlayerOpenedContainerEventHandler(IGameServer game) : IEventHandler
{
    public void Execute(IPlayer player, byte containerId, IContainer container)
    {
        SendContainerPacket(player, containerId, container);
    }

    private void SendContainerPacket(IPlayer player, byte containerId, IContainer container)
    {
        if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;

        connection.OutgoingPackets.Enqueue(new OpenContainerPacket(container, containerId));
        connection.Send();
    }
}