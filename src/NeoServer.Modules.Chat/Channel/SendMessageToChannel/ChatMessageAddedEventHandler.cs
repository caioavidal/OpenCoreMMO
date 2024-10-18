using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.Game.Chat.Channels.Contracts;
using NeoServer.Game.Common.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing.Chat;

namespace NeoServer.Modules.Chat.Channel.SendMessageToChannel;

public class ChatMessageAddedEventHandler : IEventHandler
{
    private readonly IGameServer _game;

    public ChatMessageAddedEventHandler(IGameServer game)
    {
        _game = game;
    }

    public void Execute(ISociableCreature player, IChatChannel chatChannel, SpeechType speechType, string message)
    {
        if (chatChannel is null) return;
        if (string.IsNullOrWhiteSpace(message)) return;

        foreach (var user in chatChannel.Users)
        {
            if (!_game.CreatureManager.GetPlayerConnection(user.Player.CreatureId, out var connection)) continue;
            connection.OutgoingPackets.Enqueue(new MessageToChannelPacket(player, speechType, message,
                chatChannel.Id));
            connection.Send();
        }
    }
}