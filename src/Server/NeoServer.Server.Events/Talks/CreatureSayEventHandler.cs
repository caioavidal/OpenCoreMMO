using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;

namespace NeoServer.Server.Events
{
    public class CreatureSayEventHandler
    {
        private readonly Game game;

        public CreatureSayEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(ICreature creature, SpeechType type, string message)
        {
            creature.ThrowIfNull();

            foreach (var spectator in game.Map.GetPlayersAtPositionZone(creature.Location))
            {

                if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out var connection))
                {
                    continue;
                }

                connection.OutgoingPackets.Enqueue(new CreatureSayPacket(creature, type, message));

                connection.Send();
            }
        }
    }
}
