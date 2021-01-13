using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
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
        public void Execute(IThing enemy, ICreature victim, CombatDamage damage)
        {
            foreach (var spectator in map.GetPlayersAtPositionZone(victim.Location))
            {
                if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out IConnection connection))
                {
                    continue;
                }

                var damageString = damage.ToString();

                if (victim == spectator) //myself
                {
                    connection.OutgoingPackets.Enqueue(new PlayerStatusPacket((IPlayer)victim));

                    var attackDamageType = damage.Type == DamageType.ManaDrain ? "mana points" : "health points";
                    var endMessage = enemy is null ? string.Empty : $"due to an attack by a { enemy.Name}";

                    connection.OutgoingPackets.Enqueue(new TextMessagePacket($"You lose {damageString} {attackDamageType} {endMessage}", TextMessageOutgoingType.MESSAGE_STATUS_DEFAULT));
                }

                if (enemy == spectator)
                {
                    connection.OutgoingPackets.Enqueue(new TextMessagePacket($"{victim.Name} loses {damageString} due to your attack", TextMessageOutgoingType.MESSAGE_STATUS_DEFAULT));
                }

                var damageTextColor = DamageTextColorParser.Parse(damage.Type);

                if (!damage.IsElementalDamage)
                {
                    var damageEffect = damage.Effect == EffectT.None ? DamageEffectParser.Parse(damage.Type) : damage.Effect;
                    if(damageEffect != EffectT.None) connection.OutgoingPackets.Enqueue(new MagicEffectPacket(victim.Location, damageEffect));
                }else if (damage.Effect !=  EffectT.None)
                {
                    connection.OutgoingPackets.Enqueue(new MagicEffectPacket(victim.Location, damage.Effect));
                }

                connection.OutgoingPackets.Enqueue(new AnimatedTextPacket(victim.Location, damageTextColor, damageString));
                connection.OutgoingPackets.Enqueue(new CreatureHealthPacket(victim));

                connection.Send();
            }

        }
    }
}
