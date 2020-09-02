//using NeoServer.Game.Contracts.Items;
//using NeoServer.Game.Contracts.Items.Types;
//using NeoServer.Game.Enums;
//using NeoServer.Game.Enums.Location;
//using NeoServer.Game.Enums.Location.Structs;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace NeoServer.Game.Items.Items
//{
//    public class ContainerSlotList : IEnumerable<IPickupableItem>
//    {
//        private List<IPickupableItem> _items;
//        public int SlotsUsed => _items.Count;
//        private int _capacity => _items.Capacity;
//        private IContainer _container;

//        public event RemoveItem OnItemRemoved;
//        public event AddItem OnItemAdded;
//        public event UpdateItem OnItemUpdated;

//        public ContainerSlotList(IContainer container)
//        {
//            _items = new List<IPickupableItem>(container.Capacity);
//            _container = container;
//        }

//        public Result<IPickupableItem> AddItem(IPickupableItem item, byte toSlot)
//        {

//            if (_capacity <= toSlot)
//            {
//                throw new ArgumentOutOfRangeException("Slot is bigger than capacity");
//            }

//            if (item is ICumulativeItem == false)
//            {
//                return AddItemToFront(item);
//            }

//            var cumulativeItem = item as ICumulativeItem;

//            int itemToJoinSlot = GetFirstSlotToJoin(cumulativeItem);

//            if (itemToJoinSlot >= 0 && item is ICumulativeItem cumulative)
//            {
//                //adding to a slot with a different item type

//                return TryJoinCumulativeItems(cumulative, (byte)itemToJoinSlot, out error);
//            }

//            return AddItemToFront(item, out error);
//        }

//        private int GetFirstSlotToJoin(ICumulativeItem? cumulativeItem)
//        {
//            var itemToJoinSlot = -1;
//            for (int slotIndex = 0; slotIndex < SlotsUsed; slotIndex++)
//            {
//                var itemOnSlot = _items[slotIndex];
//                if (itemOnSlot.ClientId == cumulativeItem?.ClientId && (itemOnSlot as ICumulativeItem)?.Amount < 100)
//                {
//                    itemToJoinSlot = slotIndex;
//                    break;
//                }
//            }

//            return itemToJoinSlot;
//        }

//        private Result<IPickupableItem> AddItemToFront(IPickupableItem item)
//        {
//            if (SlotsUsed >= _capacity)
//            {
//                return new Result<IPickupableItem>(InvalidOperation.FullCapacity);
//            }

//            _items.Insert(0, item);

//            if (item is IContainer container)
//                container.SetParent(_container);

//            OnItemAdded?.Invoke(item);

//            return new Result<IPickupableItem>(item);
//        }

//        public IEnumerator<IPickupableItem> GetEnumerator()
//        {
//            throw new NotImplementedException();
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}