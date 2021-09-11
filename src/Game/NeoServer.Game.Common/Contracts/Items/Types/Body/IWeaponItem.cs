using System;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Parsers;

namespace NeoServer.Game.Common.Contracts.Items.Types.Body
{
    public delegate bool AttackEnemy(ICombatActor actor, ICombatActor enemy, DamageType damageType, int minDamage,
        int maxDamage, out CombatDamage damage);

    public interface IWeapon : IBodyEquipmentItem
    {
        bool TwoHanded => Metadata.BodyPosition == Slot.TwoHanded;

        new Slot Slot => Slot.Left;
        public WeaponType Type => Metadata.WeaponType;

        bool Use(ICombatActor actor, ICombatActor enemy, out CombatAttackType combat);
    }

    public interface IWeaponItem : IWeapon
    {
        ushort Attack { get; }
        byte Defense => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.WeaponDefendValue);
        sbyte ExtraDefense => Metadata.Attributes.GetAttribute<sbyte>(ItemAttribute.ExtraDefense);

        private string ExtraDefenseText => ExtraDefense > 0 ? $"+{ExtraDefense}" :
            ExtraDefense < 0 ? $"-{ExtraDefense}" : string.Empty;

        private string AtkText => $"(Atk:{Attack} physical, {ElementalDamageText} Def: {Defense} {ExtraDefenseText})";

        string ElementalDamageText => ElementalDamage is not null
            ? $"+ {ElementalDamage.Item2} {DamageTypeParser.Parse(ElementalDamage.Item1)},"
            : string.Empty;

        Tuple<DamageType, byte> ElementalDamage { get; }
        string IItem.LookText => $"{Metadata.Article} {Metadata.Name} {AtkText}";
        string IThing.InspectionText => $"{LookText}";
    }
}