using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Networking.Packets.Outgoing.Player;

namespace NeoServer.Modules.ItemManagement.ContainerManagement.CloseContainer;

public class PlayerClosedContainerEventHandler : IEventHandler
{
    private readonly IGameServer _game;

    public PlayerClosedContainerEventHandler(IGameServer game)
    {
        _game = game;
    }

    public void Execute(IPlayer player, byte containerId, IContainer container)
    {
        if (!_game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;

        connection.OutgoingPackets.Enqueue(new CloseContainerPacket(containerId));
        connection.Send();
    }
}