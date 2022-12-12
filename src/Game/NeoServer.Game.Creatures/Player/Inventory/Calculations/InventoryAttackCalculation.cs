using System;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;

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

    internal static ulong CalculateTotalMoney(this Inventory inventory, ICoinTypeStore coinTypeStore)
    {
        uint total = 0;

        var inventoryMap = inventory.BackpackSlot?.Map;
        var coinTypes = coinTypeStore.All;

        if (coinTypes is null) return total;
        if (inventoryMap is null) return total;

        foreach (var coinType in coinTypes)
        {
            if (coinType is null) continue;
            if (!inventoryMap.TryGetValue(coinType.TypeId, out var coinAmount)) continue;

            var worthMultiplier = coinType?.Attributes?.GetAttribute<uint>(ItemAttribute.Worth) ?? 0;
            total += worthMultiplier * coinAmount;
        }

        return total;
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