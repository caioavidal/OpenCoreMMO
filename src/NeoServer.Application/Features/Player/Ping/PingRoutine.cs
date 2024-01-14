using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Application.Features.Player.Ping;

public static class PingRoutine
{
    private const int PING_INTERVAL = 5000;
    private const int CONNECTION_LOST_INTERVAL = 60000;

    public static void Execute(IPlayer player, IGameServer game)
    {
        if (player.IsDead) return;

        var now = DateTime.Now.Ticks;

        if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;

        var remainingTime = TimeSpan.FromTicks(now - connection.LastPingRequest).TotalMilliseconds;

        if (remainingTime >= PING_INTERVAL)
        {
            connection.LastPingRequest = now;

            connection.Send(new PingPacket());
        }

        var noPongTime = TimeSpan.FromTicks(now - connection.LastPingResponse).TotalMilliseconds;

        if (noPongTime >= CONNECTION_LOST_INTERVAL)
            player.Logout(true);
    }
}