﻿using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Common.Contracts.Network;
using NeoServer.Application.Common.PacketHandler;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Application.Features.Party.RevokeInvite;

public class PlayerRevokeInvitePartyPacketHandler : PacketHandler
{
    private readonly IGameServer _game;

    public PlayerRevokeInvitePartyPacketHandler(IGameServer game)
    {
        _game = game;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var creatureId = message.GetUInt32();

        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;
        if (!_game.CreatureManager.TryGetPlayer(creatureId, out var invitedPlayer) ||
            !_game.CreatureManager.IsPlayerLogged(invitedPlayer))
        {
            connection.Send(new TextMessagePacket("Revoked player is not online.", TextMessageOutgoingType.Small));
            return;
        }

        _game.Dispatcher.AddEvent(new Event(() => player.PlayerParty.RevokePartyInvite(invitedPlayer)));
    }
}