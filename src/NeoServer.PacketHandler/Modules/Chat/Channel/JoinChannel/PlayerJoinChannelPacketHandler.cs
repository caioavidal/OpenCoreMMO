using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.Modules.Chat;
using NeoServer.Modules.Chat.Channel.JoinChannel;
using NeoServer.Networking.Packets.Incoming.Chat;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.PacketHandler.Modules.Chat.Channel.JoinChannel;

public class PlayerJoinChannelPacketHandler(IGameServer game, IChatModule chatModule) : PacketHandler
{
    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var channelPacket = new OpenChannelPacket(message);
        if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        chatModule.ExecuteCommandAsync(new JoinChannelCommand(player, channelPacket.ChannelId));
    }
}