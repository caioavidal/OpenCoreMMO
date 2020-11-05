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

                var damageEffect = DamageEffectParser.Parse(attack.DamageType);

                if (attack is IAreaAttack areaAttack)
                {
                    foreach (var location in areaAttack.AffectedArea)
                    {
                        connection.OutgoingPackets.Enqueue(new MagicEffectPacket(location, damageEffect));
                    }
                }
                //else
                //{
                //    connection.OutgoingPackets.Enqueue(new MagicEffectPacket(victim.Location, damageEffect));
                //}

                if (attack is IDistanceCombatAttack distanceAttack)
                {
                    var effect = (byte)distanceAttack.ShootType;
                    connection.OutgoingPackets.Enqueue(new DistanceEffectPacket(creature.Location, victim.Location, effect));
                }
                connection.Send();
            }
        }


    }
}