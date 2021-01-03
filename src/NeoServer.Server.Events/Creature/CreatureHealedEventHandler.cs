using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events.Creature
{
    public class CreatureHealedEventHandler
    {
        private readonly IMap map;
        private readonly Game game;

        public CreatureHealedEventHandler(IMap map, Game game)
        {
            this.map = map;
            this.game = game;
        }
        public void Execute(ICreature creature, ushort amount)
        {
            foreach (var spectator in map.GetPlayersAtPositionZone(creature.Location))
            {
                if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out IConnection connection))
                {
                    continue;
                }

                if (creature == spectator) //myself
                {
                    connection.OutgoingPackets.Enqueue(new PlayerStatusPacket((IPlayer)creature));
                }

                connection.OutgoingPackets.Enqueue(new CreatureHealthPacket(creature));

                connection.Send();
            }
        }
    }
}
