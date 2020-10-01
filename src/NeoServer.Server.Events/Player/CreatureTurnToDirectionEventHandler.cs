using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums.Location;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;

namespace NeoServer.Server.Events
{
    public class CreatureTurnedToDirectionEventHandler
    {
        private readonly IMap map;
        private readonly Game game;

        public CreatureTurnedToDirectionEventHandler(IMap map, Game game)
        {
            this.map = map;
            this.game = game;
        }
        public void Execute(ICreature creature, Direction direction)
        {
            foreach (var spectatorId in map.GetPlayersAtPositionZone(creature.Location))
            {
                var isSpectator = !(creature.CreatureId == spectatorId);

                if (!game.CreatureManager.GetPlayerConnection(spectatorId, out var connection))
                {
                    continue;
                }

                connection.OutgoingPackets.Enqueue(new TurnToDirectionPacket(creature, direction));

                connection.Send();

            }

        }
    }
}
