﻿using NeoServer.Application.Common.Contracts;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Networking.Packets.Outgoing;

namespace NeoServer.Application.Features.Player.Events;

public class NotificationSentEventHandler : IEventHandler
{
    private readonly IGameServer game;

    public NotificationSentEventHandler(IGameServer game)
    {
        this.game = game;
    }

    public void Execute(IThing thing, string message, NotificationType notificationType)
    {
        if (thing is not IPlayer player) return;
        if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;

        var type = notificationType switch
        {
            NotificationType.Description => TextMessageOutgoingType.Description,
            NotificationType.Information => TextMessageOutgoingType.MESSAGE_EVENT_LEVEL_CHANGE,
            _ => TextMessageOutgoingType.Description
        };

        connection.OutgoingPackets.Enqueue(new TextMessagePacket(message,
            type));
        connection.Send();
    }
}