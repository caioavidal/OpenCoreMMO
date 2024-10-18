using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.Modules.Chat;
using NeoServer.Modules.Chat.Channel.OpenChannelList;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.PacketHandler.Modules.Chat.Channel.OpenChannelList;

public class PlayerChannelListPacketHandler(IGameServer game, IChatModule chatModule) : PacketHandler
{
    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        chatModule.ExecuteCommandAsync(new OpenChannelListCommand(player, connection));
    }
}