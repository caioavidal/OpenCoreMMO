using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.Modules.Chat;
using NeoServer.Modules.Chat.Channel.ExitNpcChannel;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.PacketHandler.Modules.Chat.Channel.ExitNpcChannel;

public class PlayerCloseNpcChannelPacketHandler(IGameServer game, IChatModule chatModule) : PacketHandler
{
    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;
        chatModule.ExecuteCommandAsync(new ExitNpcChannelCommand(player));
    }
}