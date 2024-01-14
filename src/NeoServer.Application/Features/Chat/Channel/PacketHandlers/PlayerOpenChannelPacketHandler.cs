using NeoServer.Application.Common.PacketHandler;
using NeoServer.Game.Chat.Channels.Contracts;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Infrastructure.Thread;
using NeoServer.Networking.Packets.Incoming.Chat;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Application.Features.Chat.Channel.PacketHandlers;

public class PlayerOpenChannelPacketHandler : PacketHandler
{
    private readonly IChatChannelStore _chatChannelStore;
    private readonly IGameServer _game;

    public PlayerOpenChannelPacketHandler(IGameServer game, IChatChannelStore chatChannelStore)
    {
        _game = game;
        _chatChannelStore = chatChannelStore;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var channelPacket = new OpenChannelPacket(message);
        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        IChatChannel channel = null;
        if (_chatChannelStore.Get(channelPacket.ChannelId) is { } publicChannel)
            channel = publicChannel;
        if (player.Channels.PersonalChannels?.FirstOrDefault(x => x.Id == channelPacket.ChannelId) is
            { } personalChannel) channel = personalChannel;
        if (player.Channels.PrivateChannels?.FirstOrDefault(x => x.Id == channelPacket.ChannelId) is
            { } privateChannel) channel = privateChannel;

        if (channel is null) return;

        _game.Dispatcher.AddEvent(new Event(() => player.Channels.JoinChannel(channel)));
    }
}