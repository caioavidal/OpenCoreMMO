using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events.Creature
{
    public class CreatureChangedVisibilityEventHandler
    {
        private readonly IMap map;
        private readonly Game game;

        public CreatureChangedVisibilityEventHandler(IMap map, Game game)
        {
            this.map = map;
            this.game = game;
        }
        public void Execute(IWalkableCreature creature)
        {
            foreach (var spectator in map.GetPlayersAtPositionZone(creature.Location))
            {
                if (spectator == creature) continue; 
                

                if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out IConnection connection)) continue;

                if (!creature.Tile.TryGetStackPositionOfThing((IPlayer)spectator, creature, out byte stackPostion)) continue;

                if (creature.IsInvisible && !spectator.CanSee(creature))
                {
                    connection.OutgoingPackets.Enqueue(new RemoveTileThingPacket(creature.Tile, stackPostion));
                }
                else
                {
                    connection.OutgoingPackets.Enqueue(new AddAtStackPositionPacket(creature, stackPostion));
                    connection.OutgoingPackets.Enqueue(new AddCreaturePacket((IPlayer)spectator, creature));
                }

                connection.Send();
            }
        }
    }
}