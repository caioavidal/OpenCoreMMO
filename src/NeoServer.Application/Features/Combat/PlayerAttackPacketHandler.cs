using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Common.PacketHandler;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.Application.Features.Combat;

public class PlayerAttackPacketHandler : PacketHandler
{
    private readonly IGameServer _game;

    public PlayerAttackPacketHandler(IGameServer game)
    {
        _game = game;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var targetId = message.GetUInt32();

        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        if (targetId == 0)
        {
            _game.Dispatcher.AddEvent(new Event(() => player.StopAttack()));
            return;
        }

        if (!_game.CreatureManager.TryGetCreature(targetId, out var creature)) return;

        _game.Scheduler.AddEvent(new SchedulerEvent(200, () =>
            player.SetAttackTarget(creature)));
    }
}