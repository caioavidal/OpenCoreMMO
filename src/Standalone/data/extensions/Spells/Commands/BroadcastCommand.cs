﻿using System;
using System.Text.RegularExpressions;
using NeoServer.Application.Common;
using NeoServer.Application.Common.Contracts;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;

namespace NeoServer.Extensions.Spells.Commands;

public class BroadcastCommand : CommandSpell
{
    public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
    {
        var ctx = IoC.GetInstance<IGameCreatureManager>();
        error = InvalidOperation.NotPossible;

        if (!HasAnyParameter) return false;

        var regex = new Regex("^(\\w+).\"(.+)\"$", RegexOptions.Compiled, TimeSpan.FromSeconds(1));
        var match = regex.Match(Params[0].ToString());

        if (match.Groups.Count == 3)
        {
            var (color, message) = (match.Groups[1].Value, match.Groups[2].Value);

            foreach (var player in ctx.GetAllLoggedPlayers())
            {
                if (player is null)
                    continue;

                if (ctx.GetPlayerConnection(player.CreatureId, out var connection) is false) continue;

                connection.OutgoingPackets.Enqueue(new TextMessagePacket(message,
                    GetTextMessageOutgoingTypeFromColor(color)));
                connection.Send();
            }

            error = InvalidOperation.None;
            return true;
        }

        return false;
    }

    private TextMessageOutgoingType GetTextMessageOutgoingTypeFromColor(string color)
    {
        return color switch
        {
            "white" => TextMessageOutgoingType.MESSAGE_EVENT_LEVEL_CHANGE,
            "red" => TextMessageOutgoingType.MESSAGE_STATUS_WARNING,
            "green" => TextMessageOutgoingType.Description,
            _ => TextMessageOutgoingType.Description
        };
    }
}