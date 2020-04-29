using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Enums.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeoServer.Game.Items.Items
{
    public class ContainerItem : MoveableItem, IContainerItem, IItem
    {
        public event RemoveItem OnItemRemoved;
        public event AddItem OnItemAdded;
        public event UpdateItem OnItemUpdated;
        public byte SlotsUsed { get; private set; }
        public IContainerItem Parent { get; private set; }
        public bool HasParent => Parent != null;
        public byte Capacity { get; }
        public List<IItem> Items { get; }
        public IItem this[int index] => Items[index];

        public ContainerItem(IItemType type) : base(type)
        {
            Capacity = type.Attributes.GetAttribute<byte>(Enums.ItemAttribute.Capacity);

            Items = new List<IItem>(Capacity);
        }

        public void SetParent(IContainerItem container)
        {
            Parent = container;
        }

        public static bool IsApplicable(IItemType type) => type.Group == Enums.ItemGroup.GroundContainer ||
            type.Attributes.GetAttribute(Enums.ItemAttribute.Type)?.ToLower() == "container";

        public bool GetContainerAt(byte index, out IContainerItem container)
        {
            container = null;
            if (Items[index] is IContainerItem)
            {
                container = Items[index] as IContainerItem;
                return true;
            }

            return false;
        }

        private InvalidOperation AddItem(IItem item, byte slot)
        {

            if (Capacity <= slot)
            {
                throw new ArgumentOutOfRangeException("Slot is bigger than capacity");
            }

            var itemToJoinSlot = Items.Select(i => i.ClientId).ToList().IndexOf(item.ClientId);

            if (itemToJoinSlot >= 0 && item is ICumulativeItem cumulative)
            {
                //adding to a slot with a different item type

                return JoinCumulativeItems(cumulative, (byte)itemToJoinSlot);
            }

            return AddItemToFront(item);
        }

        private InvalidOperation JoinCumulativeItems(ICumulativeItem item, byte itemToJoinSlot)
        {
            var itemToUpdate = Items[itemToJoinSlot] as ICumulativeItem;

            if (itemToUpdate.Amount == 100)
            {
                return AddItemToFront(item);
            }

            itemToUpdate.TryJoin(ref item);

            OnItemUpdated?.Invoke(itemToJoinSlot, itemToUpdate); //send update notif of the source item

            if (item != null) //item was joined on first item and remains with a amount
            {
                return AddItemToFront(item);
            }
            return InvalidOperation.None;
        }

        private InvalidOperation AddItemToFront(IItem item)
        {
            var error = InvalidOperation.None;

            if (SlotsUsed >= Capacity)
            {
                return InvalidOperation.FullCapacity;
            }

            Items.Insert(0, item);
            SlotsUsed++;

            OnItemAdded?.Invoke(item);

            return error;
        }

        private void AddItemToChild(IItem item, IContainerItem child)
        {
            child.TryAddItem(item, (byte)(child.Capacity - 1), out var error);

            if (item is IContainerItem container)
            {
                container.SetParent(child);
            }
        }
        public bool TryAddItem(IItem item, byte? slot = null) => TryAddItem(item, (byte)(Capacity - 1), out var error);
        public bool TryAddItem(IItem item, byte slot, out InvalidOperation error)
        {
            error = InvalidOperation.None;

            var itemOnSlot = Items.ElementAtOrDefault(slot);

            if (itemOnSlot is IContainerItem container)
            {
                AddItemToChild(item, container);
                return true;
            }

            error = AddItem(item, slot);
            return error == InvalidOperation.None;
        }

        public void MoveItem(byte fromSlotIndex, byte toSlotIndex, byte amount)
        {
            var item = RemoveItem(fromSlotIndex, amount) as ICumulativeItem;

            var itemOnSlot = Items.ElementAtOrDefault(toSlotIndex);

            if (itemOnSlot?.ClientId == item.ClientId)
            {
                JoinCumulativeItems(item, toSlotIndex);
            }
            else
            {
                AddItemToFront(item);
            }
        }
        public void MoveItem(byte fromSlotIndex, byte toSlotIndex)
        {
            if (GetContainerAt(toSlotIndex, out var container))
            {
                var item = RemoveItem(fromSlotIndex);
                AddItemToChild(item, container);
            }
            if (fromSlotIndex > toSlotIndex)
            {
                var item = RemoveItem(fromSlotIndex);
                AddItemToFront(item);
            }
        }
        public IItem RemoveItem(byte slotIndex)
        {
            var item = Items[slotIndex];

            Items.RemoveAt(slotIndex);

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

            OnItemUpdated?.Invoke(slotIndex, item);
            return newItem;
        }
    }
}
