using System;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Creatures.Players;

namespace NeoServer.Game.Creatures.Player.Inventory.Calculations;

public static class InventoryCalculation
{
    internal static ushort CalculateTotalAttack(this Inventory inventory)
    {
        ushort attack = 0;

        switch (inventory.Weapon)
        {
            case IWeaponItem weapon:
                return weapon.AttackPower;
            case IDistanceWeapon distance:
            {
                attack += distance.ExtraAttack;
                if (inventory.Ammo != null) attack += distance.ExtraAttack;
                break;
            }
        }

        return attack;
    }
    
    internal static byte CalculateAttackRange(this InventoryMap inventoryMap)
    {
        var rangeLeft = 0;
        var rangeRight = 0;
        const int twoHanded = 0;

        if (inventoryMap.GetItem<IAmmoEquipment>(Slot.Left) is { } leftWeapon)
            rangeLeft = leftWeapon.Range;

        if (inventoryMap.GetItem<IAmmoEquipment>(Slot.Right) is { } rightWeapon)
            rangeRight = rightWeapon.Range;

        if (inventoryMap.GetItem<IAmmoEquipment>(Slot.TwoHanded) is { } twoHandedWeapon)
            rangeRight = twoHandedWeapon.Range;

        return (byte)Math.Max(Math.Max(rangeLeft, rangeRight), twoHanded);
    }
}