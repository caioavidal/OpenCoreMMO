using System;
using System.Collections;
using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Parsers;
using NeoServer.Game.Items.Bases;

namespace NeoServer.Game.Items.Items.Weapons;

public class MeleeWeapon : Equipment, IWeaponItem, IUsableOnItem
{
    public MeleeWeapon(IItemType itemType, Location location) : base(itemType, location)
    {
        //AllowedVocations  todo
    }

    protected override string PartialInspectionText
    {
        get
        {
            var defense = Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Defense);
            var extraDefense = Metadata.Attributes.GetAttribute<sbyte>(ItemAttribute.ExtraDefense);

            var extraDefenseText = extraDefense > 0 ? $" +{extraDefense}" :
                extraDefense < 0 ? $" -{extraDefense}" : string.Empty;

            var elementalDamageText = ElementalDamage is not null && ElementalDamage.Item2 > 0
                ? $" + {ElementalDamage.Item2} {DamageTypeParser.Parse(ElementalDamage.Item1)},"
                : ",";

            return $"Atk: {AttackPower}{elementalDamageText} Def: {defense}{extraDefenseText}";
        }
    }

    public sbyte ExtraDefense => Metadata.Attributes.GetAttribute<sbyte>(ItemAttribute.ExtraDefense);
    public virtual bool CanUseOn(ushort[] items, IItem onItem)
    {
        return items is not null && ((IList)items).Contains(onItem.Metadata.TypeId);
    }

    public virtual bool CanUseOn(IItem onItem)
    {
        var useOnItems = Metadata.OnUse?.GetAttributeArray<ushort>(ItemAttribute.UseOn);

        return useOnItems is not null && ((IList)useOnItems).Contains(onItem.Metadata.TypeId);
    }

    public override bool CanBeDressed(IPlayer player)
    {
        if (Guard.IsNullOrEmpty(Vocations)) return true;

        foreach (var vocation in Vocations)
            if (vocation == player.VocationType)
                return true;

        return false;
    }

    public ushort AttackPower => Metadata.Attributes.GetAttribute<ushort>(ItemAttribute.Attack);

    public Tuple<DamageType, byte> ElementalDamage => Metadata.Attributes.GetWeaponElementDamage();

    public bool Attack(ICombatActor actor, ICombatActor enemy, out CombatAttackResult combatResult)
    {
        combatResult = new CombatAttackResult(DamageType.Melee);

        if (actor is not IPlayer player) return false;

        var result = false;

        if (AttackPower > 0)
        {
            var maxDamage = player.CalculateAttackPower(0.085f, AttackPower);
            var combat = new CombatAttackValue(actor.MinimumAttackPower,
                maxDamage, DamageType.Melee);
            if (MeleeCombatAttack.CalculateAttack(actor, enemy, combat, out var damage))
            {
                enemy.ReceiveAttack(actor, damage);
                result = true;
            }
        }

        if (ElementalDamage is null) return result;

        {
            var combat = new CombatAttackValue(actor.MinimumAttackPower,
                player.CalculateAttackPower(0.085f, ElementalDamage.Item2), ElementalDamage.Item1);

            if (MeleeCombatAttack.CalculateAttack(actor, enemy, combat, out var damage))
            {
                enemy.ReceiveAttack(actor, damage);
                result = true;
            }
        }

        return result;
    }

    public void OnMoved(IThing to)
    {
    }

    public static bool IsApplicable(IItemType type)
    {
        return type.Group is ItemGroup.MeleeWeapon;
    }
}