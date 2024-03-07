using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Common.Contracts.Network;
using NeoServer.Application.Common.PacketHandler;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Application.Features.Player.Walk;

public class PlayerAutoWalkPacketHandler : PacketHandler
{
    private readonly IGameServer _game;

    public PlayerAutoWalkPacketHandler(IGameServer game)
    {
        _game = game;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var autoWalk = new AutoWalkPacket(message);

        if (_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player))
            _game.Dispatcher.AddEvent(new Event(() => player.WalkTo(autoWalk.Steps)));
    }
}