using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Weapons;
using NeoServer.Game.Common.Contracts.Items.Weapons.Attributes;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Item.Items.Weapons;

namespace NeoServer.Game.Creature.Player.Inventory.Calculations;

internal static class InventoryAttackCalculation
{
    internal static ushort CalculateTotalAttack(this Inventory inventory)
    {
        ushort attack = 0;

        var weapon = inventory.Weapon;
        
        if (weapon is IHasAttack hasAttack)
        {
            attack += hasAttack.WeaponAttack.AttackPower;
        }

        if (weapon is IHasAttackBonus hasAttackBonus)
        {
            attack += hasAttackBonus.AttackBonus;
        }

        if (weapon is INeedsAmmo needsAmmo && needsAmmo.CanShootAmmunition(inventory.Ammo))
        {
            attack += inventory.Ammo.WeaponAttack.AttackPower;
        }

        return attack;
    }

    internal static ElementalDamage CalculateTotalElementalAttack(this Inventory inventory)
    {
        ushort attack = 0;

        var weapon = inventory.Weapon;

        var damageType = DamageType.None;

        if (weapon is IHasAttack hasAttack)
        {
            attack += hasAttack.WeaponAttack.ElementalDamage.AttackPower;
            damageType = hasAttack.WeaponAttack.ElementalDamage.DamageType;
        }

        if (weapon is INeedsAmmo needsAmmo && needsAmmo.CanShootAmmunition(inventory.Ammo))
        {
            attack += inventory.Ammo.WeaponAttack.ElementalDamage.AttackPower;
            damageType = inventory.Ammo.WeaponAttack.ElementalDamage.DamageType;
        }
        
        return new ElementalDamage(damageType, (byte)attack);
    }

    internal static byte CalculateAttackRange(this InventoryMap inventoryMap)
    {
        if (inventoryMap.GetItem<IDistanceWeapon>(Slot.Left) is { } leftWeapon)
            return leftWeapon.Range;

        if (inventoryMap.GetItem<IThrowableWeapon>(Slot.Left) is { } rightWeapon)
            return rightWeapon.Range;

        return 0;
    }
}