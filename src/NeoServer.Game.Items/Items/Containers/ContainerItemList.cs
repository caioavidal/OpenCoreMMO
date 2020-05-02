using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Enums.Location;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace NeoServer.Game.Items.Items.Containers
{
    public class ContainerItemList : IEnumerable<IItem>
    {

        public event RemoveItem OnItemRemoved;
        public event AddItem OnItemAdded;
        public event UpdateItem OnItemUpdated;
        public byte SlotsUsed { get; private set; }

        private List<IItem> _items;
        private ushort _capacity;

        public ContainerItemList(byte capacity)
        {
            _items = new List<IItem>();
            _capacity = capacity;
        }
        public InvalidOperation AddItem(IItem item, byte slot)
        {
            if (_capacity <= slot)
            {
                throw new ArgumentOutOfRangeException("Slot is bigger than capacity");
            }

            var itemToJoinSlot = _items.Select(i => i.ClientId).ToList().IndexOf(item.ClientId);

            if (itemToJoinSlot >= 0 && item is ICumulativeItem cumulative)
            {
                //adding to a slot with a different item type
                return JoinCumulativeItems(cumulative, (byte)itemToJoinSlot);
            }

            return AddItemToFront(item);
        }

        
        public InvalidOperation JoinCumulativeItems(ICumulativeItem item, byte itemToJoinSlot)
        {
            if (item is null) return InvalidOperation.None;

            var itemToUpdate = _items[itemToJoinSlot] as ICumulativeItem;


            if (itemToUpdate?.Amount == 100)
            {
                return AddItemToFront(item);
            }

            itemToUpdate?.TryJoin(ref item);

            OnItemUpdated?.Invoke(itemToJoinSlot, itemToUpdate); //send update notif of the source item

            if (item != null) //item was joined on first item and remains with a amount
            {
                return AddItemToFront(item);
            }
            return InvalidOperation.None;
        }

        public InvalidOperation AddItemToFront(IItem item)
        {
            var error = InvalidOperation.None;

            if (SlotsUsed >= _capacity)
            {
                return InvalidOperation.FullCapacity;
            }

            _items.Insert(0, item);
            SlotsUsed++;

            //if (item is IContainer container)
            //{
            //    container.SetParent(this);
            //}

            OnItemAdded?.Invoke(item);

            return error;
        }

        public void AddItemToChild(IItem item, IContainer child)
        {
            child.TryAddItem(item, (byte)(child.Capacity - 1), out var error);

            if (item is IContainer container)
            {
                container.SetParent(child);
            }
        }

        public IItem RemoveItem(byte slotIndex)
        {
            var item = _items[slotIndex];

            _items.RemoveAt(slotIndex);

            SlotsUsed--;

            OnItemRemoved?.Invoke(slotIndex, item);
            return item;
        }
        public IItem RemoveItem(byte slotIndex, byte amount)
        {
            var item = _items[slotIndex];

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

        public void MoveItem(byte fromSlotIndex, byte toSlotIndex, byte amount)
        {
            var item = RemoveItem(fromSlotIndex, amount) as ICumulativeItem;

            var itemOnSlot = _items.ElementAtOrDefault(toSlotIndex);

            if (itemOnSlot?.ClientId == item.ClientId)
            {
                JoinCumulativeItems(item, toSlotIndex);
            }
            else
            {
                AddItemToFront(item);
            }
        }
        //public void MoveItem(byte fromSlotIndex, byte toSlotIndex)
        //{
        //    if (GetContainerAt(toSlotIndex, out var container))
        //    {
        //        var item = RemoveItem(fromSlotIndex);
        //        AddItemToChild(item, container);
        //    }
        //    if (fromSlotIndex > toSlotIndex)
        //    {
        //        var item = RemoveItem(fromSlotIndex);
        //        AddItemToFront(item);
        //    }
        //}

        public IEnumerator<IItem> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
    }
}
