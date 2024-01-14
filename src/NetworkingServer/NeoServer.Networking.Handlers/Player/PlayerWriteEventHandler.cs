﻿using NeoServer.Application.Common.PacketHandler;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Infrastructure.InMemory;
using NeoServer.Infrastructure.Thread;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Handlers.Player;

public class PlayerWriteEventHandler : PacketHandler
{
    private readonly IGameServer _game;

    public PlayerWriteEventHandler(IGameServer game)
    {
        _game = game;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var writeTextPacket = new WriteTextPacket(message);

        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        if (!ItemTextWindowStore.Get(player, writeTextPacket.WindowTextId, out var item)) return;

        if (item is not IReadable readable) return;

        _game.Dispatcher.AddEvent(new Event(() =>
            player.Write(readable, writeTextPacket.Text)));
    }
}