using System;
using System.Text;
using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Combat.Calculations;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Bases;

namespace NeoServer.Game.Items.Items.Weapons;

public class DistanceWeapon : Equipment, IDistanceWeapon
{
    public DistanceWeapon(IItemType type, Location location) : base(type, location)
    {
    }

    protected override string PartialInspectionText
    {
        get
        {
            var range = Range > 0 ? $"Range: {Range}" : string.Empty;
            var atk = ExtraAttack > 0 ? $"Atk: {ExtraAttack:+#}" : string.Empty;
            var hit = ExtraHitChance != 0 ? $"Hit% {ExtraHitChance:+#;-#}" : string.Empty;

            if (Guard.AllNullOrEmpty(range, atk, hit)) return string.Empty;

            var stringBuilder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(range)) stringBuilder.Append($"{range}, ");
            if (!string.IsNullOrWhiteSpace(atk)) stringBuilder.Append($"{atk}, ");
            if (!string.IsNullOrWhiteSpace(hit)) stringBuilder.Append($"{hit}, ");

            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            return stringBuilder.ToString();
        }
    }

    public override bool CanBeDressed(IPlayer player)
    {
        if (Guard.IsNullOrEmpty(Vocations)) return true;

        foreach (var vocation in Vocations)
            if (vocation == player.VocationType)
                return true;

        return false;
    }

    public byte ExtraAttack => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Attack);
    public sbyte ExtraHitChance => Metadata.Attributes.GetAttribute<sbyte>(ItemAttribute.HitChance);
    public byte Range => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Range);

    public CombatAttackParams GetAttackParameters(ICombatActor actor, ICombatActor enemy)
    {
        if (actor is not IPlayer player) return null;

        if (player.Inventory[Slot.Ammo] is not IAmmoEquipment ammo) return null;

        var distance = (byte)actor.Location.GetSqmDistance(enemy.Location);

        var hitChance =
            (byte)(DistanceHitChanceCalculation.CalculateFor2Hands(player.GetSkillLevel(player.SkillInUse), distance) +
                   ExtraHitChance);

        var missed = DistanceCombatAttack.MissedAttack(hitChance);

        var combatParams = new CombatAttackParams
        {
            ShootType = ammo.ShootType,
            Missed = missed
        };

        var physicalAttackPower = ammo.Attack + (ammo.Attack == 0 ? 0 : ExtraAttack);
        var elementalAttackPower =
            ammo.ElementalDamage?.Item2 ?? 0 + (ammo.ElementalDamage?.Item2 == 0 ? 0 : ExtraAttack);

        var attackPower = physicalAttackPower + elementalAttackPower;

        var maxDamage = player.CalculateAttackPower(0.09f, (ushort)physicalAttackPower);

        var physicalDamage = CalculatePhysicalAttack(player, enemy, maxDamage, physicalAttackPower, ammo.Metadata.DamageType);
        UpdateDamage(physicalAttackPower, attackPower, physicalDamage);

        if (ammo.ElementalDamage is not null)
        {
            var elementalDamage = CalculateElementalAttack(player, enemy, ammo, maxDamage, elementalAttackPower);
            UpdateDamage(elementalAttackPower, attackPower, elementalDamage);

            combatParams.Damages = new[] { physicalDamage, elementalDamage };

            Ammo.PrepareAttack?.Invoke(ammo, actor, enemy, combatParams);

            return combatParams;
        }

        combatParams.Damages = new[] { physicalDamage };
        Ammo.PrepareAttack?.Invoke(ammo, actor, enemy, combatParams);

        return combatParams;
    }

    private CombatDamage CalculatePhysicalAttack(IPlayer player, ICombatActor enemy, ushort maxDamage, int attackPower,
        DamageType damageType)
    {
        var damage = new CombatDamage();
        if (attackPower <= 0) return damage;
        
        var combat = new CombatAttackCalculationValue(player.MinimumAttackPower, maxDamage, Range, damageType);

        DistanceCombatAttack.CalculateAttack(player, enemy, combat, out damage);

        return damage;
    }

    private CombatDamage CalculateElementalAttack(IPlayer player, ICombatActor enemy, IAmmoEquipment ammo,
        ushort maxDamage, int attackPower)
    {
        var damage = new CombatDamage();
        if (attackPower <= 0) return damage;

        var combat =
            new CombatAttackCalculationValue(player.MinimumAttackPower, maxDamage, Range, ammo.ElementalDamage.Item1);

        DistanceCombatAttack.CalculateAttack(player, enemy, combat, out damage);

        return damage;
    }

    private void UpdateDamage(int weaponAttackPower, int attackPower, CombatDamage damage)
    {
        var attackPowerPercentageFromTotal = 100 - weaponAttackPower * 100 / attackPower;
        var realDamage = (ushort)(damage.Damage - damage.Damage * ((double)attackPowerPercentageFromTotal / 100));

        damage.SetNewDamage(realDamage);
    }

    public bool CanAttack(IPlayer aggressor, ICombatActor victim)
    {
        return aggressor.Inventory[Slot.Ammo] is IAmmoEquipment ammo &&
               ammo.AmmoType == Metadata.AmmoType &&
               DistanceCombatAttack.CanAttack(aggressor, victim, Range) &&
               ammo.Amount > 0;
    }

    public void PreAttack(IPlayer aggressor, ICombatActor victim)
    {
        if (aggressor.Inventory[Slot.Ammo] is not IAmmoEquipment ammo) return;
        if (ammo.Amount == 0) return;

        ammo.Throw();
    }

    public static Func<IAmmoEquipment, IPlayer, ICombatActor, bool> PostAttackFunction { get; set; }
    public void PostAttack(IPlayer aggressor, ICombatActor victim)
    {
        if (aggressor.Inventory[Slot.Ammo] is not IAmmoEquipment ammo) return;

        PostAttackFunction?.Invoke(ammo, aggressor, victim);
    }

    public void OnMoved(IThing to)
    {
    }

    public static bool IsApplicable(IItemType type)
    {
        return type.Group is ItemGroup.DistanceWeapon;
    }
}