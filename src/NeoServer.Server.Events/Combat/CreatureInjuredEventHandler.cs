using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Effects.Explosion;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Item;
using NeoServer.Game.Parsers.Effects;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events
{
    public class CreatureInjuredEventHandler
    {
        private readonly IMap map;
        private readonly Game game;

        public CreatureInjuredEventHandler(IMap map, Game game)
        {
            this.map = map;
            this.game = game;
        }
        public void Execute(ICreature enemy, ICreature victim, ICombatAttack attack, ushort healthDamage)
        {
            foreach (var spectatorId in map.GetPlayersAtPositionZone(victim.Location))
            {
                if (!game.CreatureManager.GetPlayerConnection(spectatorId, out IConnection connection))
                {
                    continue;
                }

                if (victim.CreatureId == spectatorId) //myself
                {
                    connection.OutgoingPackets.Enqueue(new PlayerStatusPacket((IPlayer)victim));
                }

                var healthDamageString = healthDamage.ToString();

                if (enemy.CreatureId == spectatorId)
                {
                    connection.OutgoingPackets.Enqueue(new TextMessagePacket($"{victim.Name} loses {healthDamageString} due to your attack", TextMessageOutgoingType.MESSAGE_STATUS_DEFAULT));
                }

                var damageTextColor = DamageTextColorParser.Parse(attack.DamageType);
                var effect = DamageEffectParser.Parse(attack.DamageType);

                if (attack is IDistanceAreaCombatAttack areaAttack)
                {
                    var locations = ExplosionEffect.Create(areaAttack.Radius);

                    foreach (var location in locations)
                    {
                        connection.OutgoingPackets.Enqueue(new MagicEffectPacket(victim.Location + location, effect));
                    }
                }
                else
                {
                    connection.OutgoingPackets.Enqueue(new MagicEffectPacket(victim.Location, effect));
                }

                connection.OutgoingPackets.Enqueue(new AnimatedTextPacket(victim.Location, damageTextColor, healthDamageString));
                connection.OutgoingPackets.Enqueue(new CreatureHealthPacket(victim));

                connection.Send();
            }

        }
    }
}
