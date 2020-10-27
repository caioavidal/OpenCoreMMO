using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Events.Combat
{
    public class MonsterDefendEventHandler
    {
        private readonly Game game;

        public MonsterDefendEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(ICreature creature, ICombatDefense defense)
        {

            foreach (var spectatorId in game.Map.GetPlayersAtPositionZone(creature.Location))
            {
                if (!game.CreatureManager.GetPlayerConnection(spectatorId, out IConnection connection))
                {
                    continue;
                }

                connection.OutgoingPackets.Enqueue(new MagicEffectPacket(creature.Location, defense.Effect));
                connection.Send();
            }
        }
    }
}
