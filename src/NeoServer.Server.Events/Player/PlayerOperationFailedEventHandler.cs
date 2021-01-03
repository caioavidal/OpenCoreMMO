using NeoServer.Networking.Packets.Outgoing;

namespace NeoServer.Server.Events
{
    public class PlayerOperationFailedEventHandler
    {
        private readonly Game game;
        public PlayerOperationFailedEventHandler(Game game) => this.game = game;
        public void Execute(uint playerId, string message)
        {
            if (game.CreatureManager.GetPlayerConnection(playerId, out var connection))
            {
                connection.OutgoingPackets.Enqueue(new TextMessagePacket(message, TextMessageOutgoingType.MESSAGE_STATUS_DEFAULT));
                connection.Send();
            }
        }
    }
}
