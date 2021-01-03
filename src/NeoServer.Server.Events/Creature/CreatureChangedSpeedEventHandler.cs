using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Server.Events.Creature
{
    public class CreatureChangedSpeedEventHandler
    {
        private readonly IMap map;
        private readonly Game game;

        public CreatureChangedSpeedEventHandler(IMap map, Game game)
        {
            this.map = map;
            this.game = game;
        }
        public void Execute(IWalkableCreature creature, ushort speed)
        {
            foreach (var spectator in map.GetPlayersAtPositionZone(creature.Location))
            {
                if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out IConnection connection))
                {
                    continue;
                }
                connection.OutgoingPackets.Enqueue(new CreatureChangeSpeedPacket(creature.CreatureId, speed));
                connection.Send();
            }
        }
    }
}
