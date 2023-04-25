using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Player;

public class PlayerAttackHandler : PacketHandler
{
    private readonly IGameServer _game;

    public PlayerAttackHandler(IGameServer game)
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