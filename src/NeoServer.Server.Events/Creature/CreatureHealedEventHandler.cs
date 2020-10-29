using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Items;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;

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
            foreach (var spectatorId in map.GetPlayersAtPositionZone(creature.Location))
            {
                if (!game.CreatureManager.GetPlayerConnection(spectatorId, out IConnection connection))
                {
                    continue;
                }

                if (creature.CreatureId == spectatorId) //myself
                {
                    connection.OutgoingPackets.Enqueue(new PlayerStatusPacket((IPlayer)creature));
                }

                connection.OutgoingPackets.Enqueue(new CreatureHealthPacket(creature));

                connection.Send();
            }
        }
    }
}
