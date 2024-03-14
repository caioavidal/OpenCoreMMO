using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Common.PacketHandler;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.Application.Features.Session.Ping;

public sealed class PingResponsePacketHandler(IGameServer game) : PacketHandler
{
    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        game.Dispatcher.AddEvent(new Event(() => connection.LastPingResponse = DateTime.Now.Ticks));
    }
}