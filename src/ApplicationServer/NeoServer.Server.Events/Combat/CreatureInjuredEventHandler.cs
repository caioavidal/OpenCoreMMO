using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Effects.Parsers;
using NeoServer.Game.Common.Item;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Networking.Packets.Outgoing.Creature;
using NeoServer.Networking.Packets.Outgoing.Effect;
using NeoServer.Networking.Packets.Outgoing.Player;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Combat;

public class CreatureInjuredEventHandler
{
    private readonly IGameServer game;
    private readonly IMap map;

    public CreatureInjuredEventHandler(IMap map, IGameServer game)
    {
        this.map = map;
        this.game = game;
    }

    public void Execute(IThing enemy, ICreature victim, CombatDamage damage)
    {
        foreach (var spectator in map.GetPlayersAtPositionZone(victim.Location))
        {
            if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out var connection)) continue;

            var damageString = damage.ToString();

            if (ReferenceEquals(victim, spectator)) //myself
            {
                connection.OutgoingPackets.Enqueue(new PlayerStatusPacket((IPlayer)victim));

                var attackDamageType = damage.Type == DamageType.ManaDrain ? "mana points" : "health points";
                var endMessage = enemy is null ? string.Empty : $"due to an attack by a {enemy.Name}";

                connection.OutgoingPackets.Enqueue(new TextMessagePacket(
                    $"You lose {damageString} {attackDamageType} {endMessage}",
                    TextMessageOutgoingType.MESSAGE_STATUS_DEFAULT));
            }

            if (ReferenceEquals(enemy, spectator))
                connection.OutgoingPackets.Enqueue(new TextMessagePacket(
                    $"{victim.Name} loses {damageString} due to your attack",
                    TextMessageOutgoingType.MESSAGE_STATUS_DEFAULT));

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

            connection.Send();
        }
    }
}