using NeoServer.Application.Common.PacketHandler;
using NeoServer.Infrastructure.Thread;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Handlers.Player;

public class StopAllActionsPacketHandler : PacketHandler
{
    private readonly IGameServer _game;

    public StopAllActionsPacketHandler(IGameServer game)
    {
        _game = game;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        if (_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player))
            _game.Dispatcher.AddEvent(new Event(() => player.StopAllActions()));
    }
}