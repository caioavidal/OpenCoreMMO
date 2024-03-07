using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Common.Contracts.Network;
using NeoServer.Application.Common.PacketHandler;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Application.Features.Chat.Channel.ExitNpcChannel;

public class PlayerCloseNpcChannelPacketHandler : PacketHandler
{
    private readonly IGameServer _game;

    public PlayerCloseNpcChannelPacketHandler(IGameServer game)
    {
        _game = game;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        foreach (var creature in _game.Map.GetCreaturesAtPositionZone(player.Location))
            if (creature is INpc npc)
                _game.Dispatcher.AddEvent(new Event(() => npc.StopTalkingToCustomer(player)));
    }
}