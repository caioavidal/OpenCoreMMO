using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace NeoServer.Game.Items.Items
{
    public class Container : MoveableItem, IContainer, IItem
    {
        public event RemoveItem OnItemRemoved;
        public event AddItem OnItemAdded;
        public event UpdateItem OnItemUpdated;
        public event Move OnContainerMoved;

        public byte SlotsUsed { get; private set; }
        public bool IsFull => SlotsUsed >= Capacity;
        public byte? Id { get; private set; }
        public IThing Parent { get; private set; }
        public bool HasParent => Parent != null;
        public byte Capacity => Metadata.Attributes.GetAttribute<byte>(Common.ItemAttribute.Capacity);
        public List<IItem> Items { get; }
        public IItem this[int index] => Items.Count > index ? Items[index] : null;
        public bool HasItems => SlotsUsed > 0;
        public byte LastFreeSlot => IsFull ? 0 : SlotsUsed;
        public int FreeSlotsCount => Capacity - SlotsUsed;
        private ContainerStore Store;
        public IThing Root
        {
            get
            {
                IThing root = this;
                while (root is IContainer container && container.Parent is not null) root = container.Parent;
                return root;
            }
        }

        public override void OnMoved() => OnContainerMoved?.Invoke(this);

        public IDictionary<ushort, uint> Map => GetContainerMap();
        private IDictionary<ushort, uint> GetContainerMap(IContainer container = null, Dictionary<ushort, uint> map = null)
        {
            map = map ?? new Dictionary<ushort, uint>();
            if (container is null) container = this;

            foreach (var item in container.Items)
            {

                if (map.TryGetValue(item.Metadata.TypeId, out var val)) map[item.Metadata.TypeId] = val + item.Amount;
                else map.Add(item.Metadata.TypeId, item.Amount);

                if (item is IContainer child) GetContainerMap(child, map);
            }

            return map;
        }

        public Container(IItemType type, Location location) : base(type, location)
        {
            Items = new List<IItem>(Capacity);
            Store = new(this);
        }
        public void SetParent(IThing thing)
        {
            Parent = thing;
            if (Parent is IPlayer player) Location = new Location(Common.Players.Slot.Backpack);
        }
        public static bool IsApplicable(IItemType type) => type.Group == ItemGroup.GroundContainer || type.Attributes.GetAttribute(ItemAttribute.Type)?.ToLower() == "container";
        private int PossibleAmountToAdd(IItem item)
        {
            if (item is not ICumulative cumulative) return IsFull ? 0 : FreeSlotsCount;

            var possibleAmountToAdd = FreeSlotsCount * 100;

            foreach (var i in Items)
            {
                if (i is ICumulative c && i.ClientId == item.ClientId) possibleAmountToAdd += c.AmountToComplete;
            }

            return possibleAmountToAdd;
        }
        public bool GetContainerAt(byte index, out IContainer container)
        {
            container = null;
            if (Items.Count > index && Items[index] is IContainer c)
            {
                container = c;
                return true;
            }

            return false;
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

        private Result AddItem(IItem item, byte slot)
        {
            if (Capacity <= slot) throw new ArgumentOutOfRangeException("Slot is bigger than capacity");

            if (item is not ICumulative) return AddItemToFront(item);

            var cumulativeItem = item as ICumulative;

            int itemToJoinSlot = GetSlotOfFirstItemNotFully(cumulativeItem);

            if (itemToJoinSlot >= 0 && item is ICumulative cumulative)
            {
                return TryJoinCumulativeItems(cumulative, (byte)itemToJoinSlot);
            }

            return AddItemToFront(item);
        }

        private int GetSlotOfFirstItemNotFully(ICumulative? cumulativeItem)
        {
            var itemToJoinSlot = -1;
            for (int slotIndex = 0; slotIndex < SlotsUsed; slotIndex++)
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

            if (itemToUpdate.Amount == 100)
            {
                return AddItemToFront(item);
            }

            itemToUpdate.TryJoin(ref item);

            OnItemUpdated?.Invoke(itemToJoinSlot, itemToUpdate, (sbyte)(amountToAdd - (item?.Amount ?? 0))); //send update notif of the source item

            if (item != null) //item was joined on first item and remains with a amount
            {
                return AddItemToFront(item);
            }
            return new Result();
        }

        private Result AddItemToFront(IItem item)
        {
            if (SlotsUsed >= Capacity)
            {
                return new Result(InvalidOperation.IsFull);
            }

            Items.Insert(0, item);
            SlotsUsed++;

            if (item is IContainer container)
            {
                container.SetParent(this);
            }

            UpdateItemsLocation();

            if (item is ICumulative cumulative)
            {
                cumulative.OnReduced += OnItemReduced;
            }

            OnItemAdded?.Invoke(item);
            return Result.Success;
        }

        private void UpdateItemsLocation(byte? containerId = null)
        {
            var index = 0;
            foreach (var i in Items)
            {
                containerId = containerId ?? Id ?? 0;
                var newLocation = Location.Container(containerId.Value, (byte)index++);
                if (i is IPickupable pickupable) pickupable.SetNewLocation(newLocation);
            }
        }

        public void OnItemReduced(ICumulative item, byte amount)
        {
            if (item.Amount == 0) RemoveItem((byte)item.Location.ContainerSlot);
            if (item.Amount > 0) OnItemUpdated?.Invoke((byte)item.Location.ContainerSlot, item, (sbyte)item.Amount);
        }

        public Result CanAddItem(IItem item, byte? slot = null)
        {
            if (item == this) return new Result(InvalidOperation.Impossible);

            if (slot is null && item is not ICumulative && IsFull) return new Result(InvalidOperation.IsFull);

            if (slot is not null && GetContainerAt(slot.Value, out var container)) return container.CanAddItem(item, slot: slot);

            if (item is ICumulative cumulative && IsFull && GetSlotOfFirstItemNotFully(cumulative) == -1) return new Result(InvalidOperation.IsFull);

            return Result.Success;
        }
        public virtual Result TryAddItem(IItem item, byte? slot = null)
        {
            if (slot.HasValue && Capacity <= slot) slot = null;

            var validation = CanAddItem(item, slot);
            if (!validation.IsSuccess) return validation;

            slot = slot ?? LastFreeSlot;

            if (GetContainerAt(slot.Value, out var container)) return container.AddItem(item, null).ResultValue;

            return AddItem(item, slot.Value);
        }

        private IItem RemoveItem(byte slotIndex)
        {
            var item = Items[slotIndex];

            Items.RemoveAt(slotIndex);

            if (item is IContainer container)
            {
                container.SetParent(null);
            }

            if (item is ICumulative cumulative)
            {
                cumulative.OnReduced -= OnItemReduced;
            }

            SlotsUsed--;

            UpdateItemsLocation();

            OnItemRemoved?.Invoke(slotIndex, item);
            return item;
        }

        public int TotalFreeSlots
        {
            get
            {
                var total = 0;
                foreach (var item in Items)
                {
                    if (item is IContainer container) total += container.TotalFreeSlots;
                }

                total += FreeSlotsCount;
                return total;
            }
        }
        public void RemoveItem(IItemType itemToRemove, byte amount)
        {
            sbyte slotIndex = -1;
            var slotsToRemove = new Stack<(IItem, byte, byte)>();// slot and amount
            foreach (var item in Items)
            {
                if (item is IContainer innerContainer)
                {
                    innerContainer.RemoveItem(itemToRemove, amount);
                }

                slotIndex++;
                if (item.Metadata.TypeId != itemToRemove.TypeId) continue;
                if (amount == 0) break;

                slotsToRemove.Push((item, (byte)slotIndex, Math.Min(item.Amount, amount)));

                if (item.Amount > amount)
                {
                    amount = 0;
                    break;
                }
                amount -= item.Amount;


            }

            while (slotsToRemove.TryPop(out var slot))
            {
                var (item, slotIndexToRemove, amountToRemove) = slot;

                RemoveItem(item, amountToRemove, slotIndexToRemove, out var removedThing);
            }
        }
        public void RemoveItem(IItem item, byte amount)
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
                        RemoveItem(item, amount, slotIndex++, out var removedThing);
                        return;
                    }
                }
            }
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
                else
                {
                    OnItemUpdated?.Invoke(slotIndex, item, (sbyte)amount);
                }
            }
            else
            {
                removedItem = RemoveItem(slotIndex);
            }

            return removedItem;
        }

        public void Clear()
        {
            SetParent(null);

            foreach (var item in Items)
            {
                if (item is IContainer container)
                {
                    DetachEvents(container);
                    container.Clear();
                }

                if (item is ICumulative cumulative) cumulative.OnReduced -= OnItemReduced;
            }

            Items.Clear();
            SlotsUsed = 0;
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
                if (item is ICumulative cumulative) stringBuilder.Append(cumulative.ToString());
                else stringBuilder.Append($"{item.Name}");

                stringBuilder.Append(", ");

                if (item is IContainer container)
                {
                    stringBuilder.Append(container.ToString());
                    stringBuilder.Append(", ");
                }
            }

            return stringBuilder.ToString();
        }
        public Result<OperationResult<IItem>> AddItem(IItem item, bool includeChildren)
        {
            Result<OperationResult<IItem>> result = new(TryAddItem(item, null).Error);
            if (result.IsSuccess) return result;

            if (includeChildren)
            {
                foreach (var currentItem in Items)
                {
                    if (currentItem is not IContainer container) continue;
                    result = container.AddItem(item, includeChildren);

                    if (result.IsSuccess) return result;
                }
            }

            return result;
        }

        public Result CanAddItem(IItem item, byte amount = 1, byte? slot = null) => CanAddItem(item, slot);
        public int PossibleAmountToAdd(IItem item, byte? toPosition = null) => PossibleAmountToAdd(item);

        public bool CanRemoveItem(IItem item) => true;

        public Result<OperationResult<IItem>> AddItem(IItem item, byte? position = null) => new(TryAddItem(item, position).Error);


        public Result<OperationResult<IItem>> RemoveItem(IItem thing, byte amount, byte fromPosition, out IItem removedThing)
        {
            amount = amount == 0 ? 1 : amount;
            removedThing = RemoveItem(fromPosition, amount);
            return new(new OperationResult<IItem>(Operation.Removed, removedThing, fromPosition));
        }
        public Result<OperationResult<IItem>> SendTo(IStore destination, IItem thing, byte amount, byte fromPosition, byte? toPosition)
        {
            if (destination is IContainer && destination == this && toPosition is not null && GetContainerAt(toPosition.Value, out var container)) return SendTo(container, thing, amount, fromPosition, null);

            return Store.SendTo(destination, thing, amount, fromPosition, toPosition);

        }

        public Result<OperationResult<IItem>> ReceiveFrom(IStore source, IItem thing, byte? toPosition) => Store.ReceiveFrom(source, thing, toPosition);

    }
}