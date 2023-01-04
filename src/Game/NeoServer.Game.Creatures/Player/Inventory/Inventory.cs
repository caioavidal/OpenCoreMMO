using System;
using System.Collections.Generic;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Creatures.Player.Inventory.Calculations;
using NeoServer.Game.Creatures.Player.Inventory.Operations;
using NeoServer.Game.Creatures.Player.Inventory.Rules;

namespace NeoServer.Game.Creatures.Player.Inventory;

public class Inventory : IInventory
{
    public Inventory(IPlayer player, IDictionary<Slot, Tuple<IPickupable, ushort>> items)
    {
        InventoryMap = new InventoryMap(this);
        Owner = player;
        
        OnItemAddedToSlot += OnItemAddedToInventorySlot;

        AddItemsToInventory(items);
    }

    internal InventoryMap InventoryMap { get; }
    private void AddItemsToInventory(IDictionary<Slot, Tuple<IPickupable, ushort>> items)
    {
        foreach (var (slot, (item, _)) in items) TryAddItemToSlot(slot, item);
    }
    
    internal IAmmoEquipment Ammo => InventoryMap.GetItem<IAmmoEquipment>(Slot.Ammo);
    internal IDefenseEquipment Shield => InventoryMap.GetItem<IDefenseEquipment>(Slot.Right);
    public IWeapon Weapon => InventoryMap.GetItem<IWeapon>(Slot.Left);
    public bool IsUsingWeapon => InventoryMap.HasItemOnSlot(Slot.Left);
    public bool HasShield => InventoryMap.HasItemOnSlot(Slot.Right);
    public ushort TotalAttack => this.CalculateTotalAttack();
    public ushort TotalDefense => InventoryMap.CalculateTotalDefense();
    public ushort TotalArmor => InventoryMap.CalculateTotalArmor();
    public byte AttackRange => InventoryMap.CalculateAttackRange();
    public ulong GetTotalMoney(ICoinTypeStore coinTypeStore) => this.CalculateTotalMoney(coinTypeStore);

    /// <summary>
    ///     Gets all items that player is wearing except the bag
    /// </summary>
    public IEnumerable<IItem> DressingItems =>
        new List<IItem>
        {
            this[Slot.Head],
            this[Slot.Necklace],
            this[Slot.Body],
            this[Slot.Right],
            this[Slot.Left],
            this[Slot.Legs],
            this[Slot.Ring],
            this[Slot.Ammo],
            this[Slot.Feet]
        };

    public IDictionary<ushort, uint> Map => InventoryMap.Map;
    public IPlayer Owner { get; }
    public IItem this[Slot slot] => InventoryMap.GetItem<IItem>(slot);

    public T TryGetItem<T>(Slot slot)
    {
        if (this[slot] is T item) return item;

        return default;
    }
    public IContainer BackpackSlot => this[Slot.Backpack] as IContainer;
    public float TotalWeight => InventoryMap.CalculateTotalWeight();

    public bool RemoveItemFromSlot(Slot slot, byte amount, out IPickupable removedItem)
    {
        var result = RemoveFromSlotOperation.Remove(this, slot, amount);
        removedItem = result.Value;

        if (result.Failed) return false;

        OnItemRemovedFromSlot?.Invoke(this, removedItem, slot, amount);
        return true;
    }

    public Result<IPickupable> TryAddItemToSlot(Slot slot, IPickupable item)
    {
        var result = AddToSlotOperation.Add(this, slot, item);

        if (result.Succeeded)
        {
            OnItemAddedToSlot?.Invoke(this, item, slot);
            return result;
        }
        
        OnFailedToAddToSlot?.Invoke(Owner, result.Error);
        return result;
    }

    public Result<bool> CanAddItemToSlot(Slot slot, IItem item)
    {
        var cannotDressFail = new Result<bool>(false, InvalidOperation.CannotDress);

        if (slot == Slot.Backpack) return CanAddToBackpackSlot(item, cannotDressFail);

        if (item is not IInventoryEquipment inventoryItem) return cannotDressFail;

        if (item is IEquipmentRequirement requirement && !requirement.CanBeDressed(Owner))
            return cannotDressFail;

        if (inventoryItem is IWeapon weapon) return CanAddWeapon(slot, cannotDressFail, weapon);

        if (slot == Slot.Right && this[Slot.Left] is IWeapon { TwoHanded: true })
            //trying to add a shield while left slot has a two handed weapon
            return new Result<bool>(false, InvalidOperation.BothHandsNeedToBeFree);

        if (inventoryItem.Slot != slot) return cannotDressFail;

        return new Result<bool>(true);
    }

    public bool CanCarryItem(IPickupable item, Slot slot, byte amount = 1)
    {
        var itemWeight = item is ICumulative c ? c.CalculateWeight(amount) : item.Weight;

        if (NeedToSwap(item, slot))
        {
            var itemOnSlot = InventoryMap.GetItem<IPickupable>(slot);

            return TotalWeight - itemOnSlot.Weight + itemWeight <= Owner.TotalCapacity;
        }

        var weight = item.Weight;

        if (item is ICumulative cumulative && slot == Slot.Ammo)
        {
            var amountToAdd = cumulative.Amount > cumulative.AmountToComplete
                ? cumulative.AmountToComplete
                : cumulative.Amount;
            weight = cumulative.CalculateWeight(amountToAdd);
        }

        var canCarry = TotalWeight + weight <= Owner.TotalCapacity;
        return canCarry;
    }

    public Result CanAddItem(IItem thing, byte amount = 1, byte? slot = null)
    {
        if (thing is not IPickupable item) return Result.NotPossible;
        if (!CanCarryItem(item, (Slot)slot, amount)) return new Result(InvalidOperation.TooHeavy);
    
        return CanAddItemToSlot((Slot)slot, item).ResultValue;
    }

    public Result<uint> CanAddItem(IItemType itemType) => AddToSlotRule.CanAddItem(this, itemType);

    public uint PossibleAmountToAdd(IItem item, byte? toPosition = null)
    {
        if (toPosition is null) return 0;

        var slot = (Slot)toPosition;

        if (slot == Slot.Backpack)
        {
            if (this[slot] is null) return 1;
            if (this[slot] is IContainer container) return container.PossibleAmountToAdd(item);
        }

        if (slot != Slot.Left && slot != Slot.Ammo) return 1;

        if (item is not ICumulative) return 1;
        if (item is ICumulative c1 && this[slot] is IItem i && c1.ClientId != i.ClientId) return 100;
        if (item is ICumulative && this[slot] is null) return 100;
        if (item is ICumulative cumulative) return (uint)(100 - this[slot].Amount);

        return 0;
    }

    public bool CanRemoveItem(IItem item)
    {
        return true;
    }

    public Result<OperationResult<IItem>> AddItem(IItem thing, byte? position = null)
    {
        if (thing is not IPickupable item) return Result<OperationResult<IItem>>.NotPossible;

        position ??= (byte)thing.Metadata.BodyPosition;

        var swappedItem = TryAddItemToSlot((Slot)position, item);

        if (!swappedItem.Succeeded) return new Result<OperationResult<IItem>>(swappedItem.Error);

        if (swappedItem.Value is null) return Result<OperationResult<IItem>>.Success;

        return new Result<OperationResult<IItem>>(new OperationResult<IItem>(Operation.Removed, swappedItem.Value));
    }

    public Result<OperationResult<IItem>> RemoveItem(IItem thing, byte amount, byte fromPosition,
        out IItem removedThing)
    {
        removedThing = null;
        if (!RemoveItemFromSlot((Slot)fromPosition, amount, out var removedItem))
            return Result<OperationResult<IItem>>.NotPossible;

        removedThing = removedItem;
        return new Result<OperationResult<IItem>>();
    }

    private Result<bool> CanAddWeapon(Slot slot, Result<bool> cannotDressFail, IWeapon weapon)
    {
        if (slot != Slot.Left) return cannotDressFail;

        var hasShieldDressed = this[Slot.Right] != null;

        if (weapon.TwoHanded && hasShieldDressed)
            //trying to add a two handed while right slot has a shield
            return new Result<bool>(false, InvalidOperation.BothHandsNeedToBeFree);

        return new Result<bool>(true);
    }

    private Result<bool> CanAddToBackpackSlot(IItem item, Result<bool> cannotDressFail)
    {
        if (item is IPickupableContainer &&
            !InventoryMap.HasItemOnSlot(Slot.Backpack) &&
            item.Metadata.Attributes.GetAttribute(ItemAttribute.BodyPosition) == "backpack")
            return new Result<bool>(true);

        return InventoryMap.HasItemOnSlot(Slot.Backpack) ? new Result<bool>(true) : cannotDressFail;
    }

    private void OnItemAddedToInventorySlot(IInventory inventory, IPickupable item, Slot slot, byte amount)
    {
        if (item is IMovableItem movableItem) movableItem.SetOwner(Owner);
    }

    internal void OnItemReduced(ICumulative item, Slot slot, byte amount)
    {
        if (item.Amount == 0)
        {
            RemoveItemFromSlot(slot, amount, out var removedItem);
            return;
        }

        OnItemRemovedFromSlot?.Invoke(this, item, slot, amount);
    }

    internal (IPickupable, ushort) SwapItem(Slot slot, IPickupable item)
    {
        // var itemToSwap = InventoryMap.GetItem(slot);
        // InventoryMap.Update(slot, item, item.ClientId);
        //
        // if (item is ICumulative cumulative)
        //     cumulative.OnReduced += (itemReduced, amount) => OnItemReduced(itemReduced, slot, amount);
        //
        // return itemToSwap;
        
        var returnedItem = SwapOperation.SwapItem(this, slot, item).Value;
        
        if (item is ICumulative cumulative)
            cumulative.OnReduced += (itemReduced, amount) => OnItemReduced(itemReduced, slot, amount);

        return (returnedItem, returnedItem.ClientId);
    }

    private bool NeedToSwap(IPickupable itemToAdd, Slot slotDestination)
    {
        if (InventoryMap.GetItem<IPickupable>(slotDestination) is not {} itemOnSlot) return false;
        
        if (itemToAdd is ICumulative cumulative && itemOnSlot.ClientId == cumulative.ClientId)
            //will join
            return false;

        if (slotDestination == Slot.Backpack)
            // will add item to container
            return false;

        return true;
    }
    
    #region Events
    public event AddItemToSlot OnItemAddedToSlot;
    public event RemoveItemFromSlot OnItemRemovedFromSlot;
    public event FailAddItemToSlot OnFailedToAddToSlot;
    #endregion
}