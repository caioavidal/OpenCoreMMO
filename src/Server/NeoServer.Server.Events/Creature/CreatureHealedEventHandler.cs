using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;

namespace NeoServer.Server.Events.Creature
{
    public class CreatureHealedEventHandler
    {
        private readonly IGameServer game;
        private readonly IMap map;

        public CreatureHealedEventHandler(IMap map, IGameServer game)
        {
            this.map = map;
            this.game = game;
        }

        public void Execute(ICreature creature, ushort amount)
        {
            foreach (var spectator in map.GetPlayersAtPositionZone(creature.Location))
            {
                if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out var connection)) continue;

                if (creature == spectator) //myself
                    connection.OutgoingPackets.Enqueue(new PlayerStatusPacket((IPlayer) creature));

                connection.OutgoingPackets.Enqueue(new CreatureHealthPacket(creature));

                connection.Send();
            }
        }
    }
}