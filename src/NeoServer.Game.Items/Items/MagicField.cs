using NeoServer.Game.Common;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Conditions;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Parsers;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Parsers.Effects;
using System;

namespace NeoServer.Game.Items.Items
{
    public struct MagicField : IItem, IMagicField
    {
        public MagicField(IItemType type, Location location)
        {
            Metadata = type;
            Location = location;
        }
        public Location Location { get; set; }
        public IItemType Metadata { get; }
        public byte DamageCount => Metadata.Attributes.GetInnerAttributes(ItemAttribute.Field)?.GetAttribute<byte>(ItemAttribute.Count) ?? 0;
        public DamageType DamageType => DamageTypeParser.Parse(Metadata.Attributes.GetAttribute(ItemAttribute.Field));
        public int Interval => Metadata.Attributes.GetInnerAttributes(ItemAttribute.Field)?.GetAttribute<int>(ItemAttribute.Ticks) ?? 10000;
        public MinMax Damage
        {
            get
            {
                var attributes = Metadata.Attributes.GetInnerAttributes(ItemAttribute.Field);
                var values = attributes.GetAttributeArray(ItemAttribute.Damage);

                if ((values?.Length ?? 0) < 2) return new MinMax(0, 0);

                int.TryParse((string)values[0], out var value1);
                int.TryParse((string)values[1], out var value2);

                return new MinMax(Math.Min(value1, value2), Math.Max(value1, value2));
            }
        }

        public static bool IsApplicable(IItemType type) => type.Attributes.GetAttribute(ItemAttribute.Type) == "magicfield";

        public void CauseDamage(ICreature toCreature)
        {
            if (toCreature is not ICombatActor actor) return;

            var damages = Damage;

            if (damages.Max == 0) return;
            var conditionType = ConditionTypeParser.Parse(DamageType);
            actor.ReceiveAttack(this, new CombatDamage((ushort)damages.Max, DamageType) { Effect = DamageEffectParser.Parse(DamageType) });

            if (actor.HasCondition(conditionType, out var condition) && condition is DamageCondition damageCondition)
            {
                if (DamageCount == 0) damageCondition.Start(toCreature, (ushort)damages.Min, (ushort)damages.Max);
                else damageCondition.Restart(DamageCount);
            }
            else
            {
                if (DamageCount == 0) actor.AddCondition(new DamageCondition(conditionType, Interval, minDamage: (ushort)damages.Min, maxDamage: (ushort) damages.Max));
                else actor.AddCondition(new DamageCondition(conditionType, Interval, amount: DamageCount, damage: (ushort)damages.Min));
            }
        }

    }
}

