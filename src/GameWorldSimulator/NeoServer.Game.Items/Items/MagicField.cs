using System;
using NeoServer.Game.Combat.Attacks.DamageConditionAttack;
using NeoServer.Game.Combat.Conditions;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Effects.Parsers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Parsers;
using NeoServer.Game.Items.Bases;

namespace NeoServer.Game.Items.Items;

public class MagicField : BaseItem, IMagicField
{
    public MagicField(IItemType type, Location location) : base(type, location)
    {
    }

    private byte DamageCount => Metadata.Attributes.GetInnerAttributes(ItemAttribute.Field)
        ?.GetAttribute<byte>(ItemAttribute.Count) ?? 0;

    private DamageType DamageType => DamageTypeParser.Parse(Metadata.Attributes.GetAttribute(ItemAttribute.Field));

    private int Interval =>
        Metadata.Attributes.GetInnerAttributes(ItemAttribute.Field)?.GetAttribute<int>(ItemAttribute.Ticks) ??
        10000;

    private MinMax Damage
    {
        get
        {
            var attributes = Metadata.Attributes.GetInnerAttributes(ItemAttribute.Field);
            if (attributes is null) return new MinMax();

            var values = attributes.GetAttributeArray(ItemAttribute.Damage);

            if ((values?.Length ?? 0) < 2) return new MinMax(0, 0);

            int.TryParse((string)values[0], out var value1);
            int.TryParse((string)values[1], out var value2);

            return new MinMax(Math.Min(value1, value2), Math.Max(value1, value2));
        }
    }

    public void CauseDamage(ICreature toCreature)
    {
        var damageConditionAttack = new DamageConditionAttack(DamageType, Damage, DamageCount, Interval);
        damageConditionAttack.CauseDamage(this, toCreature);
    }

    public static bool IsApplicable(IItemType type)
    {
        return type.Group is ItemGroup.MagicField;
    }
}