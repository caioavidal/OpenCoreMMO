using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Effects.Explosion;
using NeoServer.Game.Effects.Magical;
using NeoServer.Game.Enums.Combat.Structs;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Parsers.Effects;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Helpers;
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
        public void Execute(ICreature creature, ICreature victim, CombatAttackType attack)
        {
            foreach (var spectator in game.Map.GetPlayersAtPositionZone(creature.Location))
            {

                if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out IConnection connection))
                {
                    continue;
                }

                //if (attack is IAreaAttack areaAttack)
                //{
                //    foreach (var coordinate in areaAttack.AffectedArea)
                //    {
                //        connection.OutgoingPackets.Enqueue(new MagicEffectPacket(coordinate.Location, damageEffect));
                //    }
                //}
                //else
                //{
                //    connection.OutgoingPackets.Enqueue(new MagicEffectPacket(victim.Location, damageEffect));
                //}

                //if (attack is IDistanceCombatAttack distanceAttack)
                //{
                //    var effect = (byte)distanceAttack.ShootType;
                //    connection.OutgoingPackets.Enqueue(new DistanceEffectPacket(creature.Location, victim.Location, effect));
                //}

                if (attack.Missed)
                {
                    var index = ServerRandom.Random.Next(minValue: 0, maxValue: victim.Location.Neighbours.Length);
                    var destLocation = victim.Location.Neighbours[index];
                    if (attack.ShootType != default) connection.OutgoingPackets.Enqueue(new DistanceEffectPacket(creature.Location, destLocation, (byte)attack.ShootType));
                    connection.OutgoingPackets.Enqueue(new MagicEffectPacket(destLocation, EffectT.Puff));

                }
                else
                {
                    if (attack.ShootType != default) connection.OutgoingPackets.Enqueue(new DistanceEffectPacket(creature.Location, victim.Location, (byte)attack.ShootType));
                }



                connection.Send();
            }
        }


    }
}