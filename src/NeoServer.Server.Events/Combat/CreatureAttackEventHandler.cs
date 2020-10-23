using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Effects.Explosion;
using NeoServer.Game.Effects.Magical;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Parsers.Effects;
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
        public void Execute(ICreature creature, ICreature victim, ICombatAttack attack)
        {
            foreach (var spectatorId in game.Map.GetPlayersAtPositionZone(creature.Location))
            {

                if (!game.CreatureManager.GetPlayerConnection(spectatorId, out IConnection connection))
                {
                    continue;
                }

                byte effect = 0;

                if (attack is IDistanceCombatAttack distanceAttack)
                {
                    effect = (byte)distanceAttack.ShootType;
                }

                var damageEffect = DamageEffectParser.Parse(attack.DamageType);

                if (attack is IDistanceAreaCombatAttack areaAttack)
                {
                    var locations = ExplosionEffect.Create(areaAttack.Radius);

                    foreach (var location in locations)
                    {
                        connection.OutgoingPackets.Enqueue(new MagicEffectPacket(victim.Location + location, damageEffect));
                    }

                }
                else if(attack is IDistanceSpreadCombatAttack spreadAttack)
                {
                    foreach (var location in spreadAttack.AffectedArea)
                    {
                        connection.OutgoingPackets.Enqueue(new MagicEffectPacket(location, damageEffect));
                    }

                }
                else
                {
                    connection.OutgoingPackets.Enqueue(new MagicEffectPacket(victim.Location, damageEffect));
                }


                connection.OutgoingPackets.Enqueue(new DistanceEffectPacket(creature.Location, victim.Location, effect));
                connection.Send();
            }
        }

     
    }
}