using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Networking.Packets.Outgoing.Creature;
using NeoServer.Networking.Packets.Outgoing.Item;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Creature
{
    public class CreatureChangedVisibilityEventHandler
    {
        private readonly IGameServer game;
        private readonly IMap map;

        public CreatureChangedVisibilityEventHandler(IMap map, IGameServer game)
        {
            this.map = map;
            this.game = game;
        }

        public void Execute(IWalkableCreature creature)
        {
            foreach (var spectator in map.GetPlayersAtPositionZone(creature.Location))
            {
                if (spectator == creature) continue;

                if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out var connection)) continue;

                if (!creature.Tile.TryGetStackPositionOfThing((IPlayer) spectator, creature, out var stackPostion))
                    continue;

                if (creature.IsInvisible && !spectator.CanSee(creature))
                {
                    connection.OutgoingPackets.Enqueue(new RemoveTileThingPacket(creature.Tile, stackPostion));
                }
                else
                {
                    connection.OutgoingPackets.Enqueue(new AddAtStackPositionPacket(creature, stackPostion));
                    connection.OutgoingPackets.Enqueue(new AddCreaturePacket((IPlayer) spectator, creature));
                }

                connection.Send();
            }
        }
    }
}