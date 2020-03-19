using NeoServer.Server.Contracts.Data;
using NeoServer.Server.Model.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Data.InMemory
{
    public class ItemData : IItemData
    {
        public static Dictionary<ushort, ItemType> Items { get; private set; }
        public ItemData(Dictionary<ushort, ItemType> items)
        {
            Items = items;
        }
    }
}
