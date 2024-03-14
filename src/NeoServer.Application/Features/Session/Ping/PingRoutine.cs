using NeoServer.Application.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;
using Serilog;

namespace NeoServer.Application.Features.Session.Ping;

public sealed class PingRoutine(IGameServer gameServer, ILogger logger)
{
    private const int PING_INTERVAL = 5000;
    private const int CONNECTION_LOST_INTERVAL = 60000;

    public void Execute(IPlayer player)
    {
        if (player.IsDead) return;

        var now = DateTime.Now.Ticks;

        if (!gameServer.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;

        var remainingTime = TimeSpan.FromTicks(now - connection.LastPingRequest).TotalMilliseconds;

        if (remainingTime >= PING_INTERVAL)
        {
            connection.LastPingRequest = now;

            connection.Send(new PingPacket());
        }

        var noPongTime = TimeSpan.FromTicks(now - connection.LastPingResponse).TotalMilliseconds;

        if (noPongTime < CONNECTION_LOST_INTERVAL) return;
        
        player.Logout(true);
        logger.Debug("Player {PlayerName} logged out due to ping timeout", player.Name);
    }
}