﻿using System;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Helpers;

namespace NeoServer.Game.Creatures.Player.Inventory.Rules;

public static class AddToSlotRule
{
    public static Result CanAddItem(this Inventory inventory, Slot slot, IItem item, byte amount)
    {
        if(Guard.AnyNull(slot,item)) return Result.NotPossible;
        if(item is not IPickupable pickupableItem) return Result.NotPossible;
       
        if(!inventory.CanCarryItem(pickupableItem, slot, amount)) return new Result(InvalidOperation.TooHeavy);
        
        var canAddItemToSlot = inventory.CanAddItemToSlot(slot, item);
        return !canAddItemToSlot.Value ? new Result(canAddItemToSlot.Error) : Result.Success;
    }
    
    private static Result CanAddItemToSlot(this Inventory inventory, Slot slot, IItem item)
    {
        var cannotDressFail = Result.Fail(InvalidOperation.CannotDress);

        if (slot == Slot.Backpack) return inventory.CanAddToBackpackSlot(item);

        if (item is not IInventoryEquipment inventoryItem) return cannotDressFail;

        if (item is IEquipmentRequirement requirement && !requirement.CanBeDressed(inventory.Owner))
            return cannotDressFail;

        if (inventoryItem is IWeapon weapon) return inventory.CanAddWeapon(slot, weapon);

        if (slot == Slot.Right && inventory[Slot.Left] is IWeapon { TwoHanded: true })
            //trying to add a shield while left slot has a two handed weapon
            return Result.Fail(InvalidOperation.BothHandsNeedToBeFree);

        return inventoryItem.Slot != slot ? cannotDressFail : Result.Success;
    }
    
    public static Result<uint> CanAddItem(Inventory inventory, IItemType itemType)
    {
        if (itemType is null) return Result<uint>.NotPossible;
        if (itemType.BodyPosition == Slot.None) return new Result<uint>(InvalidOperation.NotEnoughRoom);

        var itemOnSlot = inventory[itemType.BodyPosition];
        if (itemOnSlot is not null && itemType.TypeId != itemOnSlot.Metadata.TypeId)
            return new Result<uint>(InvalidOperation.NotEnoughRoom);

        byte possibleAmountToAdd;
        if (ICumulative.IsApplicable(itemType))
        {
            var amountOnSlot = inventory[itemType.BodyPosition]?.Amount ?? 0;
            possibleAmountToAdd = (byte)Math.Abs(100 - amountOnSlot);
        }
        else
        {
            if (itemOnSlot is not null) return new Result<uint>(InvalidOperation.NotEnoughRoom);
            possibleAmountToAdd = 1;
        }

        return possibleAmountToAdd == 0 ? new Result<uint>(InvalidOperation.NotEnoughRoom) : new Result<uint>(possibleAmountToAdd);
    }
 
}