using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Results;
using NeoServer.Game.Items.Bases;
using NeoServer.Game.Items.Items.Containers.Container.Calculations;
using NeoServer.Game.Items.Items.Containers.Container.Operations;
using NeoServer.Game.Items.Items.Containers.Container.Operations.Remove;
using NeoServer.Game.Items.Items.Containers.Container.Queries;
using NeoServer.Game.Items.Items.Containers.Container.Rules;

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

    private void SubscribeToEvents() => OnItemAdded += OnItemAddedToContainer;

    public byte? Id { get; private set; }
    public byte LastFreeSlot => IsFull ? (byte)0 : SlotsUsed;
    public uint FreeSlotsCount => (uint)(Capacity - SlotsUsed);
    public byte SlotsUsed { get; internal set; }
    public uint TotalOfFreeSlots => ContainerSlotsCalculation.CalculateFreeSlots(this);
    public bool IsFull => SlotsUsed >= Capacity;
    public IThing Parent { get; internal set; }
    public bool HasParent => Parent != null;
    public byte Capacity => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Capacity);
    public List<IItem> Items { get; }
    public IItem this[int index] => Items.Count > index ? Items[index] : null;
    public bool HasItems => SlotsUsed > 0;

    public IThing RootParent => FindRootParentQuery.Find(this);

    #region Weight

    private readonly ContainerWeight _containerWeight;
    public override float Weight => _containerWeight.Weight;
    internal void OnChildWeightUpdated(float change) => _containerWeight.UpdateWeight(this, change);

    #endregion

    public virtual void ClosedBy(IPlayer player){}

    public IDictionary<ushort, uint> Map => ContainerMapBuilder.Build(this);

    public void SetParent(IThing parent) => ContainerParentOperation.SetParent(this, parent);


    public void UpdateId(byte id)
    {
        Id = id;
        UpdateItemsLocation(id);
    }

    public void RemoveId() => Id = null;

    #region Queries
    public (IItem ItemFound, IContainer Container, byte SlotIndex) GetFirstItem(ushort clientId) =>
        FindFirstItemByClientIdQuery.Find(onContainer: this, clientId);
    public bool GetContainerAt(byte index, out IContainer container) => FindContainerAtIndexQuery.Find(this, index, out container);

    #endregion

    #region Remove item

    public void RemoveItem(IItemType itemToRemove, byte amount) => RemoveByItemTypeOperation.Remove(fromContainer: this, itemToRemove, amount);
    public void RemoveItem(IItem item, byte amount) => RemoveByItemOperation.Remove(fromContainer: this, item, amount);
    public Result<OperationResultList<IItem>> RemoveItem(byte fromPosition, byte amount,  out IItem removedThing)
    {
        amount = amount == 0 ? (byte)1 : amount;
        removedThing = RemoveBySlotIndexOperation.Remove(fromContainer:this, fromPosition, amount);
        return new Result<OperationResultList<IItem>>(new OperationResultList<IItem>(Operation.Removed, removedThing,
            fromPosition));
    }
    
    #endregion

    public void Clear() => ContainerClearOperation.Clear(this);

    public Result<uint> CanAddItem(IItemType itemType) => CanAddItemToContainerRule.CanAdd(toContainer: this, itemType);
    public Result CanAddItem(IItem item, byte amount = 1, byte? slot = null) => CanAddItemToContainerRule.CanAdd(toContainer: this, item, slot);
    public uint PossibleAmountToAdd(IItem item, byte? toPosition = null) => PossibleAmountToAddCalculation.Calculate(this, item);

    #region Add item
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
    public Result<OperationResultList<IItem>> AddItem(IItem item, byte? position = null)
    {
        if (item is null) return Result<OperationResultList<IItem>>.NotPossible;

        return new Result<OperationResultList<IItem>>(TryAddItem(item, position).Error);
    }
    
    private void AddChildrenItems(IEnumerable<IItem> children)
    {
        if (children is null) return;

        foreach (var item in children.Reverse())
        {
            AddItem(item);
        }
    }
    private Result AddItem(IItem item, byte slot)
    {
        if (item is null) return Result.NotPossible;
        if (Capacity <= slot) throw new ArgumentOutOfRangeException("Slot is greater than capacity");

        if (item is not ICumulative cumulativeItem) return AddItemToFront(item);

        var itemToJoinSlot = FindSlotOfFirstItemNotFullyQuery.Find(onContainer: this, cumulativeItem);

        if (itemToJoinSlot >= 0 && cumulativeItem is { } cumulative)
            return TryJoinCumulativeItems(cumulative, (byte)itemToJoinSlot);

        return AddItemToFront(cumulativeItem);
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
    
    protected virtual Result TryAddItem(IItem item, byte? slot = null)
    {
        if (item is null) return Result.NotPossible;

        if (slot.HasValue && Capacity <= slot) slot = null;

        var validation = CanAddItemToContainerRule.CanAdd(toContainer: this, item, slot);
        if (!validation.Succeeded) return validation;

        slot ??= LastFreeSlot;

        if (GetContainerAt(slot.Value, out var container)) return container.AddItem(item).ResultValue;

        return AddItem(item, slot.Value);
    }
    #endregion
  
    public bool CanBeDressed(IPlayer player) => true;

    public override void OnMoved(IThing to)
    {
        OnContainerMoved?.Invoke(this);
        base.OnMoved(to);
    }

    public void Use(IPlayer usedBy, byte openAtIndex) => usedBy.Containers.OpenContainerAt(this, openAtIndex);

    private void OnItemAddedToContainer(IItem item, IContainer container)
    {
        if (item is IMovableItem movableItem) movableItem.SetOwner(RootParent);
    }
    
    public static bool IsApplicable(IItemType type)
    {
        return type.Group == ItemGroup.GroundContainer ||
               type.Attributes.GetAttribute(ItemAttribute.Type)?.ToLower() == "container";
    }
   
    internal void UpdateItemsLocation(byte? containerId = null)
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
        if (item.Amount == 0) RemoveBySlotIndexOperation.Remove(fromContainer: this, (byte)item.Location.ContainerSlot, amount);
        if (item.Amount > 0) OnItemUpdated?.Invoke(this, (byte)item.Location.ContainerSlot, item, (sbyte)-amount);
    }
    
 

    internal void DetachEvents(IContainer container)
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
    
    internal void InvokeItemUpdatedEvent(byte slotIndex, byte amount)=> OnItemUpdated?.Invoke(this, slotIndex, Items[slotIndex], (sbyte)-amount);
    internal void InvokeItemRemovedEvent(byte slotIndex, IItem removedItem, byte amount)=> OnItemRemoved?.Invoke(this, slotIndex, removedItem, amount);

    #endregion
}