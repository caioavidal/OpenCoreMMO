using Mediator;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Creature.Events.Player;
using NeoServer.Networking.Packets.Outgoing.Effect;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Application.Features.UseItem.EventHandlers;

public class PlayerUsedItemEventHandler : INotificationHandler<PlayerUsedItemEvent>
{
    private readonly IGameCreatureManager _creatureManager;
    private readonly IMap _map;

    public PlayerUsedItemEventHandler(IMap map, IGameCreatureManager creatureManager)
    {
        _map = map;
        _creatureManager = creatureManager;
    }

    public ValueTask Handle(PlayerUsedItemEvent notification, CancellationToken cancellationToken)
    {
        notification.Deconstruct(out _, out var item, out var onThing);

        if (item is not IUsableOn usableOn) return ValueTask.CompletedTask;

        if (usableOn.Effect == EffectT.None) return ValueTask.CompletedTask;

        foreach (var spectator in _map.GetPlayersAtPositionZone(onThing.Location))
        {
            if (!_creatureManager.GetPlayerConnection(spectator.CreatureId, out var connection)) continue;

            connection.OutgoingPackets.Enqueue(new MagicEffectPacket(onThing.Location, usableOn.Effect));
            connection.Send();
        }

        return ValueTask.CompletedTask;
    }
}