using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;

namespace NeoServer.Server.Events
{
    public class CreatureHearEventHandler
    {
        private readonly Game game;

        public CreatureHearEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(ICreature from, ISociableCreature receiver, SpeechType type, string message)
        {
            if (from is null || receiver is null || type == SpeechType.None || string.IsNullOrEmpty(message)) return;

            SendMessage(from, receiver, type, message);
            return;
        }

        private void SendMessage(ICreature from, ICreature to, SpeechType type, string message)
        {
            if (!game.CreatureManager.GetPlayerConnection(to.CreatureId, out var connection)) return;

            connection.OutgoingPackets.Enqueue(new CreatureSayPacket(from, type, message));

            connection.Send();
        }
    }
}
