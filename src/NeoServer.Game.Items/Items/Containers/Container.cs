using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public IThing Parent { get; private set; }
        public bool HasParent => Parent != null;
        public byte Capacity => Metadata.Attributes.GetAttribute<byte>(Common.ItemAttribute.Capacity);
        public List<IItem> Items { get; }
        public IItem this[int index] => Items[index];
        public bool HasItems => SlotsUsed > 0;
        public byte LastFreeSlot => IsFull ? 0 : SlotsUsed;
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

        public Container(IItemType type, Location location) : base(type, location) => Items = new List<IItem>(Capacity);
        public void SetParent(IThing thing)
        {
            Parent = thing;
            if (Parent is IPlayer player) Location = new Location(Common.Players.Slot.Backpack);
        }
        public static bool IsApplicable(IItemType type) => type.Group == ItemGroup.GroundContainer || type.Attributes.GetAttribute(ItemAttribute.Type)?.ToLower() == "container";

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
                return new Result(InvalidOperation.TooHeavy);
            }

            Items.Insert(0, item);
            SlotsUsed++;

            if (item is IContainer container)
            {
                container.SetParent(this);
            }

            var index = 0;
            foreach (var i in Items)
            {
                if (i is IPickupable pickupable) pickupable.SetNewLocation(Location.Container(0, (byte)index++));
            }

            if (item is ICumulative cumulative)
            {
                cumulative.OnReduced += OnItemReduced;
            }

            OnItemAdded?.Invoke(item);
            return Result.Success;
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

            if (slot is not null && GetContainerAt(slot.Value, out var container)) return container.CanAddItem(item, null);

            if (item is ICumulative cumulative && IsFull && GetSlotOfFirstItemNotFully(cumulative) == -1) return new Result(InvalidOperation.IsFull);

            return Result.Success;
        }
        public virtual Result TryAddItem(IItem item, byte? slot = null)
        {
            if (slot.HasValue && Capacity <= slot) slot = null;

            var validation = CanAddItem(item, slot);
            if (!validation.IsSuccess) return validation;

            slot = slot ?? LastFreeSlot;

            if (GetContainerAt(slot.Value, out var container)) return container.TryAddItem(item, null);

            return AddItem(item, slot.Value);
        }
      
        public Result MoveItem(byte fromSlotIndex, byte toSlotIndex, byte amount = 1)
        {
            var movingToContainer = GetContainerAt(toSlotIndex, out var container);
            if (movingToContainer)
            {
                IItem itemToMove = null;

                if(Items[fromSlotIndex] is ICumulative cumulative)
                {
                    itemToMove = cumulative.Clone(amount);
                }
                else
                {
                    itemToMove = Items[fromSlotIndex];
                }

                var validation = container.CanAddItem(itemToMove);
                if (!validation.IsSuccess) return validation;
            }

            var item = RemoveItem(fromSlotIndex, amount);

            if (movingToContainer) return container.TryAddItem(item, toSlotIndex);

            return TryAddItem(item);
        }

        public IItem RemoveItem(byte slotIndex)
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

            OnItemRemoved?.Invoke(slotIndex, item);
            return item;
        }
        public IItem RemoveItem(byte slotIndex, byte amount)
        {
            var item = Items[slotIndex];

            IItem removedItem = null;

            if (item is ICumulative cumulative)
            {
                var amountToReduce = Math.Min(cumulative.Amount, amount);

                removedItem = cumulative.Split(amountToReduce);

                if (cumulative.Amount == 0)
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

                if(item is ICumulative cumulative) cumulative.OnReduced -= OnItemReduced;
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
    }
}