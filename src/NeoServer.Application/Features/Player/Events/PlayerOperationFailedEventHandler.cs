using NeoServer.Application.Common.Contracts;
using NeoServer.Game.Common;
using NeoServer.Networking.Packets.Outgoing;

namespace NeoServer.Application.Features.Player.Events;

public class PlayerOperationFailedEventHandler : IEventHandler
{
    private readonly IGameServer game;

    public PlayerOperationFailedEventHandler(IGameServer game)
    {
        this.game = game;
    }

    public void Execute(uint playerId, string message)
    {
        if (!game.CreatureManager.GetPlayerConnection(playerId, out var connection)) return;

        connection.OutgoingPackets.Enqueue(new TextMessagePacket(message,
            TextMessageOutgoingType.MESSAGE_STATUS_DEFAULT));
        connection.Send();
    }

    public void Execute(uint playerId, InvalidOperation invalidOperation)
    {
        if (!game.CreatureManager.GetPlayerConnection(playerId, out var connection)) return;

        connection.OutgoingPackets.Enqueue(new TextMessagePacket(TextMessageOutgoingParser.Parse(invalidOperation),
            TextMessageOutgoingType.MESSAGE_STATUS_DEFAULT));
        connection.Send();
    }
}