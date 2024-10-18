using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.Modules.Chat;
using NeoServer.Modules.Chat.Channel.JoinPrivateChannel;
using NeoServer.Networking.Packets.Incoming.Chat;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.PacketHandler.Modules.Chat.Channel.JoinPrivateChannel;

public class PlayerOpenPrivateChannelPacketHandler(IGameServer game, IChatModule chatModule) : PacketHandler
{
    public override async void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var channel = new OpenPrivateChannelPacket(message);
        if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        chatModule.ExecuteCommandAsync(new JoinPrivateChannelCommand(player, channel.Receiver, connection));
    }
}