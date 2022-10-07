using System;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Player;

public sealed class PlayerPingResponseHandler : PacketHandler
{
    private readonly IGameServer game;

    public PlayerPingResponseHandler(IGameServer game)
    {
        this.game = game;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        game.Dispatcher.AddEvent(new Event(() => connection.LastPingResponse = DateTime.Now.Ticks));
    }
}