using NeoServer.Game.Chat.Channels.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing.Chat;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Application.Features.Chat.Channel.ExitChannel;

public class PlayerExitedChannelEventHandler: IEventHandler
{
    private readonly IGameServer _game;

    public PlayerExitedChannelEventHandler(IGameServer game)
    {
        _game = game;
    }

    public void Execute(IPlayer player, IChatChannel channel)
    {
        if (channel is null) return;
        if (player is null) return;
        if (!_game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;

        connection.OutgoingPackets.Enqueue(new PlayerCloseChannelPacket(channel.Id));
        connection.Send();
    }
}