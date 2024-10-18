using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.Game.Chat.Channels.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing.Chat;

namespace NeoServer.Modules.Chat.Channel.ExitChannel;

public class PlayerExitedChannelEventHandler(IGameServer game) : IEventHandler
{
    public void Execute(IPlayer player, IChatChannel channel)
    {
        if (channel is null) return;
        if (player is null) return;
        if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;

        connection.OutgoingPackets.Enqueue(new PlayerCloseChannelPacket(channel.Id));
        connection.Send();
    }
}