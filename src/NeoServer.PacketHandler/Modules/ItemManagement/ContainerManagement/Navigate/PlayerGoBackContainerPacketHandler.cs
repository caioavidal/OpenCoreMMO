using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.BuildingBlocks.Infrastructure.Threading.Event;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.PacketHandler.Modules.ItemManagement.ContainerManagement.Navigate;

public class PlayerGoBackContainerPacketHandler : PacketHandler
{
    private readonly IGameServer _game;

    public PlayerGoBackContainerPacketHandler(IGameServer game)
    {
        _game = game;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var containerId = message.GetByte();

        if (_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player))
            _game.Dispatcher.AddEvent(new Event(() =>
                player.Containers.GoBackContainer(containerId))); //todo create a const for 2000 expiration time
    }
}