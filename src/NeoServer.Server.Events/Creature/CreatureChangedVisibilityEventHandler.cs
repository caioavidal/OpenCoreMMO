using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (creature is IMonster)
            {
                foreach (var spectator in map.GetPlayersAtPositionZone(creature.Location))
                {
                    if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out IConnection connection)) continue;

                    if (!creature.Tile.TryGetStackPositionOfThing((IPlayer)spectator, creature, out byte stackPostion)) continue;

                    if (creature.IsInvisible)
                    {
                        connection.OutgoingPackets.Enqueue(new RemoveTileThingPacket(creature.Tile, stackPostion));
                    }
                    else
                    {
                        connection.OutgoingPackets.Enqueue(new AddAtStackPositionPacket(creature, stackPostion));
                        connection.OutgoingPackets.Enqueue(new AddCreaturePacket((IPlayer)spectator, creature));
                    }
                }
            }
            if (creature is IPlayer)
            {
                if (creature.IsInvisible)
                {
                    creature.SetTemporaryOutfit(0, 0, 0, 0, 0, 0, 0);
                }
                else
                {
                    creature.DisableTemporaryOutfit();
                }
            }
        }
    }
}