using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Networking.Packets.Outgoing.Player;

namespace NeoServer.Modules.Combat.Events;

public class CreatureStoppedAttackEventHandler : IEventHandler
{
    private readonly IGameServer game;

    public CreatureStoppedAttackEventHandler(IGameServer game)
    {
        this.game = game;
    }

    public void Execute(ICombatActor actor)
    {
        if (Guard.IsNull(actor)) return;

        if (!game.CreatureManager.GetPlayerConnection(actor.CreatureId, out var connection)) return;
        connection.OutgoingPackets.Enqueue(new CancelTargetPacket());
        connection.Send();
    }
}