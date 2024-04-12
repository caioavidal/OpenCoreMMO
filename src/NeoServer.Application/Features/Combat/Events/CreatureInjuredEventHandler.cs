using NeoServer.Application.Common.Contracts;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Effects.Parsers;
using NeoServer.Game.Common.Item;
using NeoServer.Networking.Packets.Network;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Networking.Packets.Outgoing.Creature;
using NeoServer.Networking.Packets.Outgoing.Effect;
using NeoServer.Networking.Packets.Outgoing.Player;

namespace NeoServer.Application.Features.Combat.Events;

public class CreatureInjuredEventHandler(IMap map, IGameServer game) : IEventHandler
{
    public void Execute(IThing aggressor, ICreature victim, CombatDamageList damageList)
    {
        foreach (var spectator in map.GetPlayersAtPositionZone(victim.Location))
        {
            if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out var connection)) continue;

            foreach (var damage in damageList.Damages)
            {
                if (damage.Damage <= 0) continue;
                
                var damageString = damage.ToString();
                
                var damageTextColor = DamageTextColorParser.Parse(damage.Type, victim);

                if (!damage.NoEffect)
                {
                    var damageEffect = damage.Effect == EffectT.None
                        ? DamageEffectParser.Parse(damage.Type, victim)
                        : damage.Effect;
                    if (damageEffect != EffectT.None)
                        connection.OutgoingPackets.Enqueue(new MagicEffectPacket(victim.Location, damageEffect));
                }

                connection.OutgoingPackets.Enqueue(new AnimatedTextPacket(victim.Location, damageTextColor,
                    damageString));
                connection.OutgoingPackets.Enqueue(new CreatureHealthPacket(victim));
            }

            SendTextMessage(aggressor, victim, spectator, connection, damageList);
            connection.Send();
        }
    }

    private static void SendTextMessage(IThing aggressor, ICreature victim, ICreature spectator, IConnection connection,
        CombatDamageList damageList)
    {
        var manaDrain = 0;
        var healthDamage = 0;

        foreach (var damage in damageList.Damages)
        {
            if (damage.Type == DamageType.ManaDrain)
            {
                manaDrain += damage.Damage;
                continue;
            }

            healthDamage += damage.Damage;
        }

        Span<CombatDamage> groupedDamages = manaDrain > 0
            ?
            [
                new CombatDamage((ushort)healthDamage, DamageType.Melee),
                new CombatDamage((ushort)manaDrain, DamageType.ManaDrain)
            ]
            : [new CombatDamage((ushort)healthDamage, DamageType.Melee)];

        foreach (var damage in groupedDamages)
        {
            if (ReferenceEquals(victim, spectator)) //myself
            {
                connection.OutgoingPackets.Enqueue(new PlayerStatusPacket((IPlayer)victim));

                var attackDamageType = damage.Type == DamageType.ManaDrain ? "mana points" : "health points";
                var endMessage = aggressor is null ? string.Empty : $"due to an attack by a {aggressor.Name}";

                connection.OutgoingPackets.Enqueue(new TextMessagePacket(
                    $"You lose {damage.Damage} {attackDamageType} {endMessage}",
                    TextMessageOutgoingType.MESSAGE_STATUS_DEFAULT));
            }

            if (ReferenceEquals(aggressor, spectator))
                connection.OutgoingPackets.Enqueue(new TextMessagePacket(
                    $"{victim.Name} loses {damage.Damage} due to your attack",
                    TextMessageOutgoingType.MESSAGE_STATUS_DEFAULT));
        }
    }
}