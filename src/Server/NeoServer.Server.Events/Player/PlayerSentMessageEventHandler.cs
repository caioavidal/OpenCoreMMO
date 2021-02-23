using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;

namespace NeoServer.Server.Events.Creature
{
    public class PlayerSentMessageEventHandler
    {
        private readonly Game game;

        public PlayerSentMessageEventHandler(Game game)
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
}
