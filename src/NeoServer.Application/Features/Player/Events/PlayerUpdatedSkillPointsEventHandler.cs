﻿using NeoServer.Application.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Networking.Packets.Outgoing.Player;

namespace NeoServer.Application.Features.Player.Events;

public class PlayerUpdatedSkillPointsEventHandler : IEventHandler
{
    private readonly IGameServer game;

    public PlayerUpdatedSkillPointsEventHandler(IGameServer game)
    {
        this.game = game;
    }

    public void Execute(IPlayer player, SkillType skill)
    {
        if (Guard.AnyNull(player)) return;
        if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;

        connection.OutgoingPackets.Enqueue(new PlayerSkillsPacket(player));
        connection.Send();
    }

    public void Execute(IPlayer player, SkillType skill, sbyte increased)
    {
        if (game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection))
        {
            connection.OutgoingPackets.Enqueue(new PlayerSkillsPacket(player));
            connection.Send();
        }
    }
}