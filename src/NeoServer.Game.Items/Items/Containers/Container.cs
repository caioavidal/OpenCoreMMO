using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
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
        public byte SlotsUsed { get; private set; }
        public bool IsFull => SlotsUsed >= Capacity;
        public IContainer Parent { get; private set; }
        public bool HasParent => Parent != null;
        public byte Capacity => Metadata.Attributes.GetAttribute<byte>(Enums.ItemAttribute.Capacity);
        public List<IItem> Items { get; }
        public IItem this[int index] => Items[index];

        public Container(IItemType type, Location location) : base(type, location)
        {
            if (!type.Attributes.HasAttribute(Enums.ItemAttribute.Capacity))
            {
                throw new ArgumentException("Capacity missing");
            }

            Items = new List<IItem>(Capacity);
        }

        public void SetParent(IContainer container) => Parent = container;

        public static bool IsApplicable(IItemType type) => type.Group == Enums.ItemGroup.GroundContainer ||
            type.Attributes.GetAttribute(Enums.ItemAttribute.Type)?.ToLower() == "container";

        public bool GetContainerAt(byte index, out IContainer container)
        {
            container = null;
            if (Items[index] is IContainer)
            {
                container = Items[index] as IContainer;
                return true;
            }

            return false;
        }

        private bool AddItem(IItem item, byte slot, out InvalidOperation error)
        {

            if (Capacity <= slot)
            {
                throw new ArgumentOutOfRangeException("Slot is bigger than capacity");
            }
           
            if (item is ICumulativeItem == false)
            {
                return AddItemToFront(item, out error);

            }

            var cumulativeItem = item as ICumulativeItem;

            int itemToJoinSlot = GetSlotOfFirstItemNotFully(cumulativeItem);

            if (itemToJoinSlot >= 0 && item is ICumulativeItem cumulative)
            {
                //adding to a slot with a different item type

                return TryJoinCumulativeItems(cumulative, (byte)itemToJoinSlot, out error);
            }

            return AddItemToFront(item, out error);
        }

        private int GetSlotOfFirstItemNotFully(ICumulativeItem? cumulativeItem)
        {
            var itemToJoinSlot = -1;
            for (int slotIndex = 0; slotIndex < SlotsUsed; slotIndex++)
            {
                var itemOnSlot = Items[slotIndex];
                if (itemOnSlot.ClientId == cumulativeItem?.ClientId && (itemOnSlot as ICumulativeItem)?.Amount < 100)
                {
                    itemToJoinSlot = slotIndex;
                    break;
                }
            }

            return itemToJoinSlot;
        }

        private bool TryJoinCumulativeItems(ICumulativeItem item, byte itemToJoinSlot, out InvalidOperation error)
        {
            error = InvalidOperation.None;

            var amountToAdd = item.Amount;

            var itemToUpdate = Items[itemToJoinSlot] as ICumulativeItem;

            if (itemToUpdate.Amount == 100)
            {
                return AddItemToFront(item, out error);
            }

            itemToUpdate.TryJoin(ref item);

            OnItemUpdated?.Invoke(itemToJoinSlot, itemToUpdate, (sbyte)(amountToAdd - (item?.Amount ?? 0))); //send update notif of the source item

            if (item != null) //item was joined on first item and remains with a amount
            {
                return AddItemToFront(item, out error);
            }
            return true;
        }

        private bool AddItemToFront(IItem item, out InvalidOperation error)
        {
            error = InvalidOperation.None;

            if (SlotsUsed >= Capacity)
            {
                error = InvalidOperation.FullCapacity;
                return false;
            }

            Items.Insert(0, item);
            SlotsUsed++;

            if (item is IContainer container)
            {
                container.SetParent(this);
                container.OnItemUpdated += OnItemUpdated;
                container.OnItemAdded += OnItemAdded;
                container.OnItemRemoved += OnItemRemoved;
            }

            OnItemAdded?.Invoke(item);

            return true;
        }

        

        private bool AddItemToChild(IItem item, IContainer child, out InvalidOperation error)
        {
            if (!item.IsCumulative && child.IsFull)
            {
                error = InvalidOperation.FullCapacity;
                return false;
            }
            var result = child.TryAddItem(item, (byte)(child.Capacity - 1), out error);

            if (item is IContainer container)
            {
                container.SetParent(child);
            }
            return result;
        }
        public bool TryAddItem(IItem item, byte? slot = null) => TryAddItem(item, slot ?? (byte)(Capacity - 1), out var error);
        public bool TryAddItem(IItem item, byte slot, out InvalidOperation error)
        {
            error = InvalidOperation.None;

            var itemOnSlot = Items.ElementAtOrDefault(slot);

            if (itemOnSlot is IContainer container)
            {
                return AddItemToChild(item, container, out error);
            }

            return AddItem(item, slot, out error);
        }

        public void MoveItem(byte fromSlotIndex, byte toSlotIndex, byte amount = 1)
        {
            var item = RemoveItem(fromSlotIndex, amount) as ICumulativeItem;

            var itemOnSlot = Items.ElementAtOrDefault(toSlotIndex);

            if (itemOnSlot?.ClientId == item.ClientId)
            {
                TryJoinCumulativeItems(item, toSlotIndex, out var error);
            }
            else
            {
                AddItemToFront(item, out var error);
            }
        }
        public bool MoveItem(byte fromSlotIndex, byte toSlotIndex, out InvalidOperation error)
        {
            error = InvalidOperation.None;

            if (GetContainerAt(toSlotIndex, out var container))
            {
                var item = RemoveItem(fromSlotIndex);
                return AddItemToChild(item, container, out error);
            }
            if (fromSlotIndex > toSlotIndex)
            {
                var item = RemoveItem(fromSlotIndex);
                return AddItemToFront(item, out error);
            }
            return true;
        }
        public IItem RemoveItem(byte slotIndex)
        {
            var item = Items[slotIndex];

            Items.RemoveAt(slotIndex);

            if(item is IContainer container)
            {
                container.SetParent(null);
                container.OnItemUpdated -= OnItemUpdated;
                container.OnItemAdded -= OnItemAdded;
                container.OnItemRemoved -= OnItemRemoved;
            }

            SlotsUsed--;

            OnItemRemoved?.Invoke(slotIndex, item);
            return item;
        }
        public IItem RemoveItem(byte slotIndex, byte amount)
        {
            var item = Items[slotIndex];

            IItem newItem = null;

            if (item is ICumulativeItem cumulative)
            {
                newItem = cumulative.Split(Math.Min(cumulative.Amount, amount));

                if (cumulative.Amount == 0)
                {
                    RemoveItem(slotIndex);
                    return newItem;
                }
            }

            OnItemUpdated?.Invoke(slotIndex, item, (sbyte)-amount);
            return newItem;
        }
    }
}
