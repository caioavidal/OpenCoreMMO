using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Bases;

namespace NeoServer.Game.Items.Items.Containers;

public class Container : MovableItem, IContainer
{
    public Container(IItemType type, Location location, IEnumerable<IItem> children = null) : base(
        type, location)
    {
        Items = new List<IItem>(Capacity);
        new ContainerHasItem(this);
        OnItemAdded += OnItemAddedToContainer;

        AddChildrenItems(children);
    }

    private void OnItemAddedToContainer(IItem item, IContainer container)
    {
        if (item is IMovableItem movableItem) movableItem.SetOwner(RootParent);
    }

    public byte? Id { get; private set; }
    public byte LastFreeSlot => IsFull ? (byte)0 : SlotsUsed;
    public uint FreeSlotsCount => (uint)(Capacity - SlotsUsed);
    public event RemoveItem OnItemRemoved;
    public event AddItem OnItemAdded;
    public event UpdateItem OnItemUpdated;
    public event Move OnContainerMoved;

    public byte SlotsUsed { get; private set; }
    public bool IsFull => SlotsUsed >= Capacity;
    public IThing Parent { get; private set; }
    public bool HasParent => Parent != null;
    public byte Capacity => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Capacity);
    public List<IItem> Items { get; }
    public IItem this[int index] => Items.Count > index ? Items[index] : null;
    public bool HasItems => SlotsUsed > 0;

    public IThing RootParent
    {
        get
        {
            IThing root = this;
            while (root is IContainer { Parent: { } } container) root = container.Parent;
            return root;
        }
    }

    public virtual void ClosedBy(IPlayer player)
    {
    }

    public IDictionary<ushort, uint> Map => GetContainerMap();

    public void SetParent(IThing thing)
    {
        Parent = thing;
        if (Parent is IPlayer) Location = new Location(Slot.Backpack);
        SetOwner(RootParent);
    }

    public bool GetContainerAt(byte index, out IContainer container)
    {
        container = null;
        if (Items.Count <= index || Items[index] is not IContainer c) return false;

        container = c;
        return true;
    }

    public void UpdateId(byte id)
    {
        Id = id;
        UpdateItemsLocation(id);
    }

    public void RemoveId()
    {
        Id = null;
    }

    public uint TotalOfFreeSlots
    {
        get
        {
            uint total = 0;
            foreach (var item in Items)
                if (item is IContainer container)
                    total += container.TotalOfFreeSlots;

            total += FreeSlotsCount;
            return total;
        }
    }

    public void RemoveItem(IItemType itemToRemove, byte amount) //todo: slow method
    {
        sbyte slotIndex = -1;
        var slotsToRemove = new Stack<(IItem, byte, byte)>(); // slot and amount
        foreach (var item in Items)
        {
            if (item is IContainer innerContainer) innerContainer.RemoveItem(itemToRemove, amount);

            slotIndex++;
            if (item.Metadata.TypeId != itemToRemove.TypeId) continue;
            if (amount == 0) break;

            slotsToRemove.Push((item, (byte)slotIndex, Math.Min(item.Amount, amount)));

            if (item.Amount > amount) break;

            amount -= item.Amount;
        }

        while (slotsToRemove.TryPop(out var slot))
        {
            var (item, slotIndexToRemove, amountToRemove) = slot;

            RemoveItem(item, amountToRemove, slotIndexToRemove, out var removedThing);
        }
    }

    public void RemoveItem(IItem item, byte amount) //todo: slow method
    {
        var containers = new Queue<IContainer>();
        containers.Enqueue(this);

        while (containers.TryDequeue(out var container))
        {
            byte slotIndex = 0;
            foreach (var containerItem in container.Items)
            {
                if (containerItem is IContainer innerContainer) containers.Enqueue(innerContainer);
                if (containerItem == item)
                {
                    RemoveItem(item, amount, slotIndex++, out _);
                    return;
                }
            }
        }
    }

    public (IItem, IContainer, byte) GetFirstItem(ushort clientId)
    {
        var containers = new Queue<IContainer>();
        containers.Enqueue(this);

        while (containers.TryDequeue(out var container))
        {
            byte slotIndex = 0;
            foreach (var containerItem in container.Items)
            {
                if (containerItem is IContainer innerContainer) containers.Enqueue(innerContainer);
                if (containerItem.ClientId == clientId) return (containerItem, container, slotIndex);

                slotIndex++;
            }
        }

        return (null, null, 0);
    }

    public void Clear()
    {
        SetParent(null);

        if (Items is null) return;

        while (Items.FirstOrDefault() is { } item)
        {
            switch (item)
            {
                case IContainer container:
                    DetachEvents(container);
                    container.Clear();
                    break;
                case ICumulative cumulative:
                    cumulative.OnReduced -= OnItemReduced;
                    break;
            }

            RemoveItem(item, item.Amount);
        }

        Items.Clear();
        SlotsUsed = 0;
    }

    public Result<OperationResult<IItem>> AddItem(IItem item, bool includeChildren)
    {
        Result<OperationResult<IItem>> result = new(TryAddItem(item).Error);
        if (result.Succeeded) return result;

        if (!includeChildren) return result;

        foreach (var currentItem in Items)
        {
            if (currentItem is not IContainer container) continue;
            result = container.AddItem(item, true);

            if (result.Succeeded)
            {
                return result;
            }
        }

        return result;
    }

    public Result CanAddItem(IItem item, byte amount = 1, byte? slot = null)
    {
        return CanAddItem(item, slot);
    }

    public uint PossibleAmountToAdd(IItem item, byte? toPosition = null)
    {
        return PossibleAmountToAdd(item);
    }

    public bool CanRemoveItem(IItem item)
    {
        return true;
    }

    public Result<OperationResult<IItem>> AddItem(IItem item, byte? position = null)
    {
        return new Result<OperationResult<IItem>>(TryAddItem(item, position).Error);
    }

    public Result<OperationResult<IItem>> RemoveItem(IItem thing, byte amount, byte fromPosition,
        out IItem removedThing)
    {
        amount = amount == 0 ? (byte)1 : amount;
        removedThing = RemoveItem(fromPosition, amount);
        return new Result<OperationResult<IItem>>(new OperationResult<IItem>(Operation.Removed, removedThing,
            fromPosition));
    }

    // public Result<OperationResult<IItem>> SendTo(IHasItem destination, IItem thing, byte amount, byte fromPosition,
    //     byte? toPosition)
    // {
    //     if (destination is IContainer && destination == this && toPosition is not null &&
    //         GetContainerAt(toPosition.Value, out var container))
    //         return SendTo(container, thing, amount, fromPosition, null);
    //
    //     return _hasItem.SendTo(destination, thing, amount, fromPosition, toPosition);
    // }

    /// <summary>
    ///     Checks if item cam be added to any containers within current container
    /// </summary>
    /// <param name="itemType"></param>
    /// <returns>Returns true when any amount is possible to add</returns>
    public Result<uint> CanAddItem(IItemType itemType)
    {
        if (!ICumulative.IsApplicable(itemType) && TotalOfFreeSlots > 0) return new Result<uint>(TotalOfFreeSlots);

        var containers = new Queue<IContainer>();

        containers.Enqueue(this);

        var amountPossibleToAdd = TotalOfFreeSlots * 100;

        if (Map.TryGetValue(itemType.TypeId, out var totalAmount))
        {
            var next100 = Math.Ceiling((decimal)totalAmount / 100) * 100;
            amountPossibleToAdd += (uint)(next100 - totalAmount);
        }

        return amountPossibleToAdd > 0
            ? new Result<uint>(amountPossibleToAdd)
            : new Result<uint>(InvalidOperation.NotEnoughRoom);
    }

    public bool CanBeDressed(IPlayer player)
    {
        return true;
    }

    public override void OnMoved(IThing to)
    {
        OnContainerMoved?.Invoke(this);
        base.OnMoved(to);
    }

    private void AddChildrenItems(IEnumerable<IItem> children)
    {
        if (children is null) return;

        foreach (var item in children.Reverse()) AddItem(item);
    }

    private IDictionary<ushort, uint> GetContainerMap(IContainer container = null,
        Dictionary<ushort, uint> map = null)
    {
        map ??= new Dictionary<ushort, uint>();
        container ??= this;

        foreach (var item in container.Items)
        {
            if (map.TryGetValue(item.Metadata.TypeId, out var val)) map[item.Metadata.TypeId] = val + item.Amount;
            else map.Add(item.Metadata.TypeId, item.Amount);

            if (item is IContainer child) GetContainerMap(child, map);
        }

        return map;
    }

    public static bool IsApplicable(IItemType type)
    {
        return type.Group == ItemGroup.GroundContainer ||
               type.Attributes.GetAttribute(ItemAttribute.Type)?.ToLower() == "container";
    }

    private uint PossibleAmountToAdd(IItem item)
    {
        if (item is not ICumulative) return IsFull ? 0 : FreeSlotsCount;

        var possibleAmountToAdd = FreeSlotsCount * 100;

        foreach (var i in Items)
            if (i is ICumulative c && i.ClientId == item.ClientId)
                possibleAmountToAdd += c.AmountToComplete;

        return possibleAmountToAdd;
    }

    private Result AddItem(IItem item, byte slot)
    {
        if (Capacity <= slot) throw new ArgumentOutOfRangeException("Slot is greater than capacity");

        if (item is not ICumulative cumulativeItem) return AddItemToFront(item);

        var itemToJoinSlot = GetSlotOfFirstItemNotFully(cumulativeItem);

        if (itemToJoinSlot >= 0 && cumulativeItem is { } cumulative)
            return TryJoinCumulativeItems(cumulative, (byte)itemToJoinSlot);

        return AddItemToFront(cumulativeItem);
    }

    private int GetSlotOfFirstItemNotFully(ICumulative? cumulativeItem)
    {
        var itemToJoinSlot = -1;
        for (var slotIndex = 0; slotIndex < SlotsUsed; slotIndex++)
        {
            var itemOnSlot = Items[slotIndex];
            if (itemOnSlot.ClientId == cumulativeItem?.ClientId && (itemOnSlot as ICumulative)?.Amount < 100)
            {
                itemToJoinSlot = slotIndex;
                break;
            }
        }

        return itemToJoinSlot;
    }

    private Result TryJoinCumulativeItems(ICumulative item, byte itemToJoinSlot)
    {
        var amountToAdd = item.Amount;

        var itemToUpdate = Items[itemToJoinSlot] as ICumulative;

        if (itemToUpdate.Amount == 100) return AddItemToFront(item);

        itemToUpdate.TryJoin(ref item);

        OnItemUpdated?.Invoke(itemToJoinSlot, itemToUpdate,
            (sbyte)(amountToAdd - (item?.Amount ?? 0))); //send update notif of the source item

        if (item != null) //item was joined on first item and remains with a amount
            return AddItemToFront(item);
        return new Result();
    }

    private Result AddItemToFront(IItem item)
    {
        if (SlotsUsed >= Capacity) return new Result(InvalidOperation.IsFull);
        item.Location = Location;
        Items.Insert(0, item);
        SlotsUsed++;

        if (item is IContainer container) container.SetParent(this);

        UpdateItemsLocation();

        if (item is ICumulative cumulative) cumulative.OnReduced += OnItemReduced;

        OnItemAdded?.Invoke(item, this);
        return Result.Success;
    }

    private void UpdateItemsLocation(byte? containerId = null)
    {
        var index = 0;
        foreach (var i in Items)
        {
            containerId ??= Id ?? 0;
            var newLocation = Location.Container(containerId.Value, (byte)index++);

            if (i.IsPickupable) i.Location = newLocation;
        }
    }

    private void OnItemReduced(ICumulative item, byte amount)
    {
        if (item.Amount == 0) RemoveItem((byte)item.Location.ContainerSlot);
        if (item.Amount > 0) OnItemUpdated?.Invoke((byte)item.Location.ContainerSlot, item, (sbyte)item.Amount);
    }

    private Result CanAddItem(IItem item, byte? slot = null)
    {
        if (item == this) return new Result(InvalidOperation.Impossible);

        if (slot is null && item is not ICumulative && IsFull) return new Result(InvalidOperation.IsFull);

        if (slot is not null && GetContainerAt(slot.Value, out var container))
            return container.CanAddItem(item, slot: slot);

        if (item is ICumulative cumulative && IsFull && GetSlotOfFirstItemNotFully(cumulative) == -1)
            return new Result(InvalidOperation.IsFull);

        return Result.Success;
    }

    protected virtual Result TryAddItem(IItem item, byte? slot = null)
    {
        if (slot.HasValue && Capacity <= slot) slot = null;

        var validation = CanAddItem(item, slot);
        if (!validation.IsSuccess) return validation;

        slot ??= LastFreeSlot;

        if (GetContainerAt(slot.Value, out var container)) return container.AddItem(item).ResultValue;

        return AddItem(item, slot.Value);
    }

    private IItem RemoveItem(byte slotIndex)
    {
        var item = Items[slotIndex];

        Items.RemoveAt(slotIndex);

        if (item is IContainer container) container.SetParent(null);

        if (item is ICumulative cumulative) cumulative.OnReduced -= OnItemReduced;

        SlotsUsed--;

        UpdateItemsLocation();

        OnItemRemoved?.Invoke(slotIndex, item);
        return item;
    }

    private IItem RemoveItem(byte slotIndex, byte amount)
    {
        var item = Items[slotIndex];

        IItem removedItem = null;

        if (item is ICumulative cumulative)
        {
            var amountToReduce = Math.Min(cumulative.Amount, amount);

            var amountBeforeSplit = cumulative.Amount;
            removedItem = cumulative.Split(amountToReduce);

            if (amountBeforeSplit == removedItem.Amount)
            {
                RemoveItem(slotIndex);
                return removedItem;
            }

            OnItemUpdated?.Invoke(slotIndex, item, (sbyte)amount);
        }
        else
        {
            removedItem = RemoveItem(slotIndex);
        }

        return removedItem;
    }

    private void DetachEvents(IContainer container)
    {
        container.OnItemUpdated -= OnItemUpdated;
        container.OnItemAdded -= OnItemAdded;
        container.OnItemRemoved -= OnItemRemoved;
    }

    public override string ToString()
    {
        var content = GetStringContent();
        if (string.IsNullOrWhiteSpace(content)) return "nothing";

        return content.Remove(content.Length - 2, 2);
    }

    private string GetStringContent()
    {
        var stringBuilder = new StringBuilder();

        foreach (var item in Items)
        {
            if (item is ICumulative cumulative) stringBuilder.Append(cumulative);
            else stringBuilder.Append($"{item.Name}");

            stringBuilder.Append(", ");

            if (item is IContainer container)
            {
                stringBuilder.Append(container);
                stringBuilder.Append(", ");
            }
        }

        return stringBuilder.ToString();
    }
}