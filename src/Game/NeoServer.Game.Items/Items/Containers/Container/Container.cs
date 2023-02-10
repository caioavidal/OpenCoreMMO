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
using NeoServer.Game.Common.Results;
using NeoServer.Game.Items.Bases;
using NeoServer.Game.Items.Items.Containers.Container.Operations;
using NeoServer.Game.Items.Items.Containers.Container.Queries;

namespace NeoServer.Game.Items.Items.Containers.Container;

public class Container : MovableItem, IContainer
{
    public Container(IItemType type, Location location, IEnumerable<IItem> children = null) : base(
        type, location)
    {
        _containerWeight = new ContainerWeight(this);

        Items = new List<IItem>(Capacity);

        SubscribeToEvents();

        AddChildrenItems(children);
    }

    private void SubscribeToEvents()
    {
        OnItemAdded += OnItemAddedToContainer;
    }

    public byte? Id { get; private set; }
    public byte LastFreeSlot => IsFull ? (byte)0 : SlotsUsed;
    public uint FreeSlotsCount => (uint)(Capacity - SlotsUsed);
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

    #region Weight

    private readonly ContainerWeight _containerWeight;
    public override float Weight => _containerWeight.Weight;
    internal void OnChildWeightUpdated(float change) => _containerWeight.UpdateWeight(this, change);

    #endregion

    public virtual void ClosedBy(IPlayer player)
    {
    }

    public IDictionary<ushort, uint> Map => ContainerMapBuilder.Build(this);


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

    public void RemoveId() => Id = null;

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

    #region Remove item

    public void RemoveItem(IItemType itemToRemove, byte amount) //todo: slow method
    {
        var slotsToRemove = FindItemByTypeQuery.Search(Items, itemToRemove, amount);

        while (slotsToRemove.TryPop(out var slot))
        {
            var (item, slotIndexToRemove, amountToRemove) = slot;

            RemoveItem(item, amountToRemove, slotIndexToRemove, out _);
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
                if (containerItem != item)
                {
                    slotIndex++;
                    continue;
                }

                RemoveItem(item, amount, slotIndex, out _);
                return;
            }
        }
    }

    public Result<OperationResultList<IItem>> RemoveItem(IItem thing, byte amount, byte fromPosition,
        out IItem removedThing)
    {
        amount = amount == 0 ? (byte)1 : amount;
        removedThing = RemoveItem(fromPosition, amount);
        return new Result<OperationResultList<IItem>>(new OperationResultList<IItem>(Operation.Removed, removedThing,
            fromPosition));
    }

    private IItem RemoveItem(byte slotIndex)
    {
        var amount = Items[slotIndex].Amount;
        return RemoveFromContainerOperation.RemoveItem(this, slotIndex, amount).Value;
    }
    
    private IItem RemoveItem(byte slotIndex, byte amount)
    {
        var result = RemoveFromContainerOperation.RemoveItem(this, slotIndex, amount);
        
        if (result.Failed) return null;
        
        if(result.Operation is Operation.Updated) OnItemUpdated?.Invoke(this, slotIndex, result.Value, (sbyte)-amount);
        
        if (result.Operation is Operation.Removed)
        {
            SlotsUsed--;
            UpdateItemsLocation();
            OnItemRemoved?.Invoke(this, slotIndex, result.Value, amount);
        }

        return result.Value;
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

    #endregion

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

    public Result<OperationResultList<IItem>> AddItem(IItem item, bool includeChildren)
    {
        if (item is null) return Result<OperationResultList<IItem>>.NotPossible;

        Result<OperationResultList<IItem>> result = new(TryAddItem(item).Error);
        if (result.Succeeded) return result;

        if (!includeChildren) return result;

        foreach (var currentItem in Items)
        {
            if (currentItem is not IContainer container) continue;
            result = container.AddItem(item, true);

            if (result.Succeeded) return result;
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

    public Result<OperationResultList<IItem>> AddItem(IItem item, byte? position = null)
    {
        if (item is null) return Result<OperationResultList<IItem>>.NotPossible;

        return new Result<OperationResultList<IItem>>(TryAddItem(item, position).Error);
    }


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

    public void Use(IPlayer usedBy, byte openAtIndex)
    {
        usedBy.Containers.OpenContainerAt(this, openAtIndex);
    }

    private void OnItemAddedToContainer(IItem item, IContainer container)
    {
        if (item is IMovableItem movableItem) movableItem.SetOwner(RootParent);
    }

    private void AddChildrenItems(IEnumerable<IItem> children)
    {
        if (children is null) return;

        foreach (var item in children.Reverse())
        {
            AddItem(item);
        }
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
        if (item is null) return Result.NotPossible;
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
        if (item is null) return Result.NotPossible;

        var amountToAdd = item.Amount;

        var itemToUpdate = Items[itemToJoinSlot] as ICumulative;

        if (itemToUpdate.Amount == 100) return AddItemToFront(item);

        itemToUpdate.TryJoin(ref item);

        OnItemUpdated?.Invoke(this, itemToJoinSlot, itemToUpdate,
            (sbyte)(amountToAdd - (item?.Amount ?? 0))); //send update notif of the source item

        if (item != null) //item was joined on first item and remains with a amount
            return AddItemToFront(item);
        return new Result();
    }

    private Result AddItemToFront(IItem item)
    {
        if (item is null) return Result.NotPossible;
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

            if (i is IMovableThing movableThing) movableThing.SetNewLocation(newLocation);
        }
    }

    internal void OnItemReduced(ICumulative item, byte amount)
    {
        if (item.Amount == 0) RemoveItem((byte)item.Location.ContainerSlot, amount);
        if (item.Amount > 0) OnItemUpdated?.Invoke(this, (byte)item.Location.ContainerSlot, item, (sbyte)-amount);
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
        if (item is null) return Result.NotPossible;

        if (slot.HasValue && Capacity <= slot) slot = null;

        var validation = CanAddItem(item, slot);
        if (!validation.Succeeded) return validation;

        slot ??= LastFreeSlot;

        if (GetContainerAt(slot.Value, out var container)) return container.AddItem(item).ResultValue;

        return AddItem(item, slot.Value);
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

        return content;
    }

    private string GetStringContent()
    {
        if (!Items.Any()) return null;

        var stringBuilder = new StringBuilder();

        foreach (var item in Items)
        {
            if (item is IContainer)
            {
                stringBuilder.Append(item.FullName);
                stringBuilder.Append(", ");
            }

            stringBuilder.Append(item);
            stringBuilder.Append(", ");
        }

        return stringBuilder.Remove(stringBuilder.Length - 2, 2).ToString();
    }

    #region Events

    public event RemoveItem OnItemRemoved;
    public event AddItem OnItemAdded;
    public event UpdateItem OnItemUpdated;
    public event Move OnContainerMoved;

    #endregion
}