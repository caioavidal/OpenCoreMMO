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
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Player.Inventory.Calculations;

namespace NeoServer.Game.Creatures.Player.Inventory;

public class Inventory : IInventory
{
    public Inventory(IPlayer player, IDictionary<Slot, Tuple<IPickupable, ushort>> items)
    {
        _inventoryMap = new InventoryMap(this);
        Owner = player;
        
        OnItemAddedToSlot += OnItemAddedToInventorySlot;

        AddItemsToInventory(items);
    }

    private readonly InventoryMap _inventoryMap;
    private void AddItemsToInventory(IDictionary<Slot, Tuple<IPickupable, ushort>> items)
    {
        foreach (var (slot, (item, _)) in items) TryAddItemToSlot(slot, item);
    }
    
    internal IAmmoEquipment Ammo => _inventoryMap.GetItem<IAmmoEquipment>(Slot.Ammo);
    internal IDefenseEquipment Shield => _inventoryMap.GetItem<IDefenseEquipment>(Slot.Right);
    public IWeapon Weapon => _inventoryMap.GetItem<IWeapon>(Slot.Left);
    public bool IsUsingWeapon => _inventoryMap.HasItemOnSlot(Slot.Left);
    public bool HasShield => _inventoryMap.HasItemOnSlot(Slot.Right);
    public ushort TotalAttack => this.CalculateTotalAttack();
    public ushort TotalDefense => _inventoryMap.CalculateTotalDefense();
    public ushort TotalArmor => _inventoryMap.CalculateTotalArmor();
    public byte AttackRange => _inventoryMap.CalculateAttackRange();
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

    public IDictionary<ushort, uint> Map => _inventoryMap.Map;
    public IPlayer Owner { get; }
    public IItem this[Slot slot] => _inventoryMap.GetItem<IItem>(slot);

    public T TryGetItem<T>(Slot slot)
    {
        if (this[slot] is T item) return item;

        return default;
    }
    public IContainer BackpackSlot => this[Slot.Backpack] as IContainer;
    public float TotalWeight => _inventoryMap.CalculateTotalWeight();

    public bool RemoveItemFromSlot(Slot slot, byte amount, out IPickupable removedItem)
    {
        removedItem = null;

        if (amount == 0) return false;
        if (_inventoryMap.GetItem<IPickupable>(slot) is not { } item) return false;

        if (item is ICumulative cumulative && amount < cumulative.Amount)
        {
            removedItem = cumulative.Split(amount);
        }
        else
        {
            if (item is ICumulative c) c.ClearSubscribers();

            _inventoryMap.Remove(slot);
            removedItem = item;
        }

        if (removedItem is IDressable dressable) dressable.UndressFrom(Owner);

        OnItemRemovedFromSlot?.Invoke(this, removedItem, slot, amount);
        return true;
    }

    public Result<IPickupable> TryAddItemToSlot(Slot slot, IPickupable item)
    {
        var canCarry = CanCarryItem(item, slot);

        if (!canCarry)
        {
            OnFailedToAddToSlot?.Invoke(Owner, InvalidOperation.TooHeavy);
            return new Result<IPickupable>(InvalidOperation.TooHeavy);
        }

        var canAddItemToSlot = CanAddItemToSlot(slot, item);
        if (!canAddItemToSlot.Value)
        {
            OnFailedToAddToSlot?.Invoke(Owner, canAddItemToSlot.Error);
            return new Result<IPickupable>(canAddItemToSlot.Error);
        }

        if (slot is Slot.Backpack)
        {
            if (_inventoryMap.GetItem<IPickupableContainer>(Slot.Backpack) is {} backpack)
                return new Result<IPickupable>(null,
                    (backpack).AddItem(item).Error);

            if (item is IPickupableContainer container) container.SetParent(Owner);
        }

        var slotHasItem = _inventoryMap.HasItemOnSlot(slot);

        //todo: refact
        if (slotHasItem)
        {
            (IPickupable, ushort) itemToSwap = default;

            if (item is ICumulative cumulative)
            {
                if (NeedToSwap(cumulative, slot))
                {
                    itemToSwap = SwapItem(slot, cumulative);
                }
                else
                {
                    _inventoryMap.GetItem<ICumulative>(slot).TryJoin(ref cumulative);
                    
                    if (cumulative?.Amount > 0)
                        itemToSwap = (cumulative, cumulative.ClientId);
                }

                if (itemToSwap.Item1 is ICumulative c) c.ClearSubscribers();
            }
            else
            {
                itemToSwap = SwapItem(slot, item);
            }

            if (item is IDressable dressableItem) dressableItem.DressedIn(Owner);

            if (itemToSwap.Item1 is IDressable dressableRemovedItem) dressableRemovedItem.UndressFrom(Owner);
            OnItemAddedToSlot?.Invoke(this, item, slot);
            return itemToSwap == default ? new Result<IPickupable>() : new Result<IPickupable>(itemToSwap.Item1);
        }
        else
        {
            if (item is ICumulative cumulative)
                cumulative.OnReduced += (item, amount) => OnItemReduced(item, slot, amount);
        }

        _inventoryMap.Add(slot, item, item.ClientId);

        if(item is IMovableThing movableThing) movableThing.SetNewLocation(Location.Inventory(slot));

        if (item is IDressable dressable) dressable.DressedIn(Owner);

        OnItemAddedToSlot?.Invoke(this, item, slot);
        return new Result<IPickupable>();
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
            var itemOnSlot = _inventoryMap.GetItem<IPickupable>(slot);

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

    public Result<uint> CanAddItem(IItemType itemType)
    {
        if (itemType is null) return Result<uint>.NotPossible;
        if (itemType.BodyPosition == Slot.None) return new Result<uint>(InvalidOperation.NotEnoughRoom);

        var itemOnSlot = this[itemType.BodyPosition];
        if (itemOnSlot is not null && itemType.TypeId != itemOnSlot.Metadata.TypeId)
            return new Result<uint>(InvalidOperation.NotEnoughRoom);

        byte possibleAmountToAdd;
        if (ICumulative.IsApplicable(itemType))
        {
            var amountOnSlot = this[itemType.BodyPosition]?.Amount ?? 0;
            possibleAmountToAdd = (byte)Math.Abs(100 - amountOnSlot);
        }
        else
        {
            if (itemOnSlot is not null) return new Result<uint>(InvalidOperation.NotEnoughRoom);
            possibleAmountToAdd = 1;
        }

        if (possibleAmountToAdd == 0) return new Result<uint>(InvalidOperation.NotEnoughRoom);

        return new Result<uint>(possibleAmountToAdd);
    }

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
            !_inventoryMap.HasItemOnSlot(Slot.Backpack) &&
            item.Metadata.Attributes.GetAttribute(ItemAttribute.BodyPosition) == "backpack")
            return new Result<bool>(true);

        return _inventoryMap.HasItemOnSlot(Slot.Backpack) ? new Result<bool>(true) : cannotDressFail;
    }

    private void OnItemAddedToInventorySlot(IInventory inventory, IPickupable item, Slot slot, byte amount)
    {
        if (item is IMovableItem movableItem) movableItem.SetOwner(Owner);
    }

    private void OnItemReduced(ICumulative item, Slot slot, byte amount)
    {
        if (item.Amount == 0)
        {
            RemoveItemFromSlot(slot, amount, out var removedItem);
            return;
        }

        OnItemRemovedFromSlot?.Invoke(this, item, slot, amount);
    }

    private (IPickupable, ushort) SwapItem(Slot slot, IPickupable item)
    {
        var itemToSwap = _inventoryMap.GetItem(slot);
        _inventoryMap.Update(slot, item, item.ClientId);

        if (item is ICumulative cumulative)
            cumulative.OnReduced += (itemReduced, amount) => OnItemReduced(itemReduced, slot, amount);

        return itemToSwap;
    }

    private bool NeedToSwap(IPickupable itemToAdd, Slot slotDestination)
    {
        if (_inventoryMap.GetItem<IPickupable>(slotDestination) is not {} itemOnSlot) return false;
        
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