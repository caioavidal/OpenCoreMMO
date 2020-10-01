using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Events.Combat
{
    public class CreatureAttackEventHandler
    {
        private readonly Game game;

        public CreatureAttackEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(ICreature creature, ICreature enemy, ICombatAttack attack)
        {
            foreach (var spectatorId in game.Map.GetPlayersAtPositionZone(creature.Location))
            {
                byte effect = 0;

                if (attack is IDistanceCombatAttack distanceAttack)
                {
                    effect = (byte)distanceAttack.ShootType;
                }

                if (!game.CreatureManager.GetPlayerConnection(spectatorId, out IConnection connection))
                {
                    continue;
                }

                connection.OutgoingPackets.Enqueue(new DistanceEffectPacket(creature.Location, enemy.Location, effect));
                connection.Send();
            }
        }
    }
}