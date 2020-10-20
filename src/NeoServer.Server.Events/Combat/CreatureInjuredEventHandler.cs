using Microsoft.Diagnostics.Runtime.Utilities;
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
        public void Execute(ICreature enemy, ICreature victim, ICombatAttack attack, ushort damage)
        {
            foreach (var spectatorId in map.GetPlayersAtPositionZone(victim.Location))
            {
                if (!game.CreatureManager.GetPlayerConnection(spectatorId, out IConnection connection))
                {
                    continue;
                }

                var damageString = damage.ToString();

                if (victim.CreatureId == spectatorId) //myself
                {
                    connection.OutgoingPackets.Enqueue(new PlayerStatusPacket((IPlayer)victim));

                    var attackDamageType = attack.DamageType == DamageType.ManaDrain ? "mana points" : "health points";
                    connection.OutgoingPackets.Enqueue(new TextMessagePacket($"You lose {damageString} {attackDamageType} due to an attack by a {enemy.Name}", TextMessageOutgoingType.MESSAGE_STATUS_DEFAULT));
                }


                if (enemy.CreatureId == spectatorId)
                {
                    connection.OutgoingPackets.Enqueue(new TextMessagePacket($"{victim.Name} loses {damageString} due to your attack", TextMessageOutgoingType.MESSAGE_STATUS_DEFAULT));
                }

                var damageTextColor = DamageTextColorParser.Parse(attack.DamageType);
             

                connection.OutgoingPackets.Enqueue(new AnimatedTextPacket(victim.Location, damageTextColor, damageString));
                connection.OutgoingPackets.Enqueue(new CreatureHealthPacket(victim));

                connection.Send();
            }

        }
    }
}
