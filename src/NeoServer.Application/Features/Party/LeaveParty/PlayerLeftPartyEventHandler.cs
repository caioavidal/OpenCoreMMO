﻿using NeoServer.Application.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures.Party;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Texts;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Networking.Packets.Outgoing.Party;

namespace NeoServer.Application.Features.Party.LeaveParty;

public class PlayerLeftPartyEventHandler : IEventHandler
{
    private readonly IGameServer game;

    public PlayerLeftPartyEventHandler(IGameServer game)
    {
        this.game = game;
    }

    public void Execute(IPlayer oldMember, IParty party)
    {
        if (Guard.AnyNull(oldMember, party)) return;

        game.CreatureManager.GetPlayerConnection(oldMember.CreatureId, out var oldMemberConnection);

        oldMemberConnection?.OutgoingPackets?.Enqueue(new PartyEmblemPacket(oldMember, PartyEmblem.None));
        oldMemberConnection?.OutgoingPackets?.Enqueue(new TextMessagePacket(
            !party.IsOver ? "You have left the party" : TextConstants.PARTY_HAS_BEEN_DISBANDED,
            TextMessageOutgoingType.Description));

        foreach (var member in party.Members)
        {
            if (member == oldMember) continue;

            if (!game.CreatureManager.GetPlayerConnection(member.CreatureId, out var memberConnection)) continue;

            if (!party.IsOver)
                memberConnection.OutgoingPackets.Enqueue(
                    new TextMessagePacket($"{oldMember.Name} has left the party",
                        TextMessageOutgoingType.Description));

            if (member.CanSee(oldMember.Location))
            {
                memberConnection.OutgoingPackets.Enqueue(new PartyEmblemPacket(oldMember, PartyEmblem.None));
                oldMemberConnection?.OutgoingPackets?.Enqueue(new PartyEmblemPacket(member, PartyEmblem.None));
            }

            memberConnection.Send();
        }

        oldMemberConnection?.Send();
    }
}