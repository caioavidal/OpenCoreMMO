using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Common.PacketHandler;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.Application.Features.Combat.AutoAttack;

public class PlayerAttackPacketHandler(IGameServer game) : PacketHandler
{
    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var targetId = message.GetUInt32();

        if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        if (targetId == 0)
        {
            game.Dispatcher.AddEvent(new Event(() => player.StopAttack()));
            return;
        }

        if (!game.CreatureManager.TryGetCreature(targetId, out var creature)) return;

        game.Scheduler.AddEvent(new SchedulerEvent(200, () =>
            player.SetAttackTarget(creature)));
    }
}