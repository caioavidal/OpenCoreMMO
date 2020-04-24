//using NeoServer.Game.Contracts.Items;
//using NeoServer.Server.Items;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Collections.Immutable;
//using System.Runtime.InteropServices;

//namespace NeoServer.Game.Items
//{
//    public class ItemData
//    {
//        /// <summary>
//        /// InMemory data of all game's item types
//        /// </summary>
//        /// <value></value>
//        public static ConcurrentDictionary<ushort, IItem> InMemory { get; private set; } = new ConcurrentDictionary<ushort, IItem>();

//        public static bool Add(Item item) => InMemory.TryAdd(item.Type.TypeId, item);

//        public static IItem GetItem(ushort typeId)
//        {
//            if (InMemory.TryGetValue(typeId,out var item))
//            {
//                return item;
//            }
//            var newItem = new Item(ItemTypeData.InMemory[typeId]);
//            Add(newItem);
//            return newItem;
//        }
//    }
//}