using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Talks;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;

namespace NeoServer.Server.Events
{
    public class CreatureSayEventHandler
    {
        private readonly Game game;

        public CreatureSayEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(ICreature creature, TalkType type, string message)
        {
            creature.ThrowIfNull();

            foreach (var spectatorId in game.Map.GetPlayersAtPositionZone(creature.Location))
            {

                if (!game.CreatureManager.GetPlayerConnection(spectatorId, out var connection))
                {
                    continue;
                }

                connection.OutgoingPackets.Enqueue(new CreatureSayPacket(creature, type, message));

                connection.Send();
            }
        }
    }
}
