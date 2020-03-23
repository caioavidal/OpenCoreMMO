using NeoServer.Server.Model.Items;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Text;

namespace NeoServer.Server.Items
{
    public class ItemData
    {
        public static ImmutableDictionary<ushort, ItemType> Items { get; private set; }

        public static void Load(Dictionary<ushort, ItemType> items)
        {
            if (Items == null)
            {
                Items = items.ToImmutableDictionary();
            }
        }
    }
}
