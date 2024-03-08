﻿using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Common.Contracts.Network;
using NeoServer.Application.Common.PacketHandler;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Application.Features.Party.JoinParty;

public class PlayerJoinPartyPacketHandler : PacketHandler
{
    private readonly IGameServer _game;

    public PlayerJoinPartyPacketHandler(IGameServer game)
    {
        _game = game;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var leaderId = message.GetUInt32();

        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;
        if (!_game.CreatureManager.TryGetPlayer(leaderId, out var leader) ||
            !_game.CreatureManager.IsPlayerLogged(leader))
        {
            connection.Send(new TextMessagePacket("Player is not online.", TextMessageOutgoingType.Small));
            return;
        }

        _game.Dispatcher.AddEvent(new Event(() => player.PlayerParty.JoinParty(leader.PlayerParty.Party)));
    }
}