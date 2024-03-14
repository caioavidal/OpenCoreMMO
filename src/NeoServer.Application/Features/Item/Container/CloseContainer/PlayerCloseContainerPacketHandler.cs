using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Common.PacketHandler;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.Application.Features.Item.Container.CloseContainer;

public class PlayerCloseContainerPacketHandler : PacketHandler
{
    private readonly IGameServer _game;

    public PlayerCloseContainerPacketHandler(IGameServer game)
    {
        _game = game;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var containerId = message.GetByte();
        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        _game.Dispatcher.AddEvent(new Event(() => player.Containers.CloseContainer(containerId)));
    }
}