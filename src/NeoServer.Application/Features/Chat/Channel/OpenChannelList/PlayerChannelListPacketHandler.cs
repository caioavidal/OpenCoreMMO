using NeoServer.Application.Common.PacketHandler;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Networking.Packets.Outgoing.Chat;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Application.Features.Chat.Channel.OpenChannelList;

public class PlayerChannelListPacketHandler : PacketHandler
{
    private readonly IChatChannelStore _chatChannelStore;
    private readonly IGameServer _game;

    public PlayerChannelListPacketHandler(IGameServer game, IChatChannelStore chatChannelStore)
    {
        _game = game;
        _chatChannelStore = chatChannelStore;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        var channels = _chatChannelStore.All.Where(x => x.PlayerCanJoin(player));
        channels = player.Channels.PersonalChannels is null
            ? channels
            : channels.Concat(player.Channels.PersonalChannels);
        channels = player.Channels.PrivateChannels is not { } privateChannels
            ? channels
            : channels.Concat(privateChannels);

        connection.OutgoingPackets.Enqueue(new PlayerChannelListPacket(channels.ToArray()));
        connection.Send();
    }
}