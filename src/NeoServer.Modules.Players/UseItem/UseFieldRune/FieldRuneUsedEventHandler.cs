using Mediator;
using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Item.Items.UsableItems.Runes.Events;
using NeoServer.Networking.Packets.Outgoing.Effect;

namespace NeoServer.Modules.Players.UseItem.UseFieldRune;

public class FieldRuneUsedEventHandler : INotificationHandler<FieldRuneUsedOnTileEvent>
{
    private readonly IGameCreatureManager _creatureManager;
    private readonly IMap _map;

    public FieldRuneUsedEventHandler(IMap map, IGameCreatureManager creatureManager)
    {
        _map = map;
        _creatureManager = creatureManager;
    }

    public ValueTask Handle(FieldRuneUsedOnTileEvent notification, CancellationToken cancellationToken)
    {
        var usedBy = notification.Player;
        var item = notification.Rune;
        var onTile = notification.OnTile;

        foreach (var spectator in _map.GetPlayersAtPositionZone(usedBy.Location))
        {
            if (!_creatureManager.GetPlayerConnection(spectator.CreatureId, out var connection)) continue;

            if (item.Metadata.ShootType != default)
                connection.OutgoingPackets.Enqueue(new DistanceEffectPacket(usedBy.Location, onTile.Location,
                    (byte)item.Metadata.ShootType));
            connection.Send();
        }

        return ValueTask.CompletedTask;
    }
}