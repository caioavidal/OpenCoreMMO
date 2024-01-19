using NeoServer.Application.Common.PacketHandler;
using NeoServer.Infrastructure.Thread;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Application.Features.Item.Container.Navigate;

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