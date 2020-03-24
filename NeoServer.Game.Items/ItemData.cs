using NeoServer.Game.Contracts.Item;
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
        public static ImmutableDictionary<ushort, IItemType> Items { get; private set; }

        public static void Load(Dictionary<ushort, IItemType> items)
        {
            if (Items == null)
            {
                Items = items.ToImmutableDictionary();
            }
        }
    }
}
