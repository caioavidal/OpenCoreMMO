using NeoServer.Game.Common.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing.Chat;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Application.Features.Player.Events;

public class PlayerSentMessageEventHandler: IEventHandler
{
    private readonly IGameServer game;

    public PlayerSentMessageEventHandler(IGameServer game)
    {
        this.game = game;
    }

    public void Execute(ISociableCreature from, ISociableCreature to, SpeechType speechType, string message)
    {
        if (string.IsNullOrWhiteSpace(message) || to is null || from is null) return;

        if (!game.CreatureManager.GetPlayerConnection(to.CreatureId, out var receiverConnection)) return;

        receiverConnection.OutgoingPackets.Enqueue(new PlayerSendPrivateMessagePacket(from, speechType, message));
        receiverConnection.Send();
    }
}