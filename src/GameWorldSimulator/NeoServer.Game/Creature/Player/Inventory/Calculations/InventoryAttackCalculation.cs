using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Creatures.Players;

namespace NeoServer.Game.Creature.Player.Inventory.Calculations;

internal static class InventoryAttackCalculation
{
    internal static ushort CalculateTotalAttack(this Inventory inventory)
    {
        ushort attack = 0;

        switch (inventory.Weapon)
        {
            case IWeaponItem weapon:
                return weapon.WeaponAttack.AttackPower;
            case IDistanceWeapon distance:
            {
                attack += distance.ExtraAttack;
                if (inventory.Ammo != null) attack += inventory.Ammo.WeaponAttack.AttackPower;
                break;
            }
            case null:
            {
                return 7;
            }
        }

        return attack;
    }
    
    internal static ushort CalculateTotalElementalAttack(this Inventory inventory)
    {
        ushort attack = 0;

        switch (inventory.Weapon)
        {
            case IWeaponItem weapon:
                return weapon.WeaponAttack.ElementalDamage.AttackPower;
            case IDistanceWeapon distance:
            {
                attack += distance.ExtraAttack;
                if (inventory.Ammo != null) attack += inventory.Ammo.WeaponAttack.ElementalDamage.AttackPower;
                break;
            }
            default:
            {
                return 0;
            }
        }

        return attack;
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