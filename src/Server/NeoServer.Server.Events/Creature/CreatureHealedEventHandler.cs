using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Networking.Packets.Outgoing.Creature;
using NeoServer.Networking.Packets.Outgoing.Player;
using NeoServer.Server.Common.Contracts;

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