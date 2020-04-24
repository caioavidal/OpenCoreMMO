using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Enums.Location.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Items.Items
{
    public class Item : IItem
    {
        public Item(IItemType metadata, Location location)
        {
            Metadata = metadata;
            Location = location;
        }
        public IItemType Metadata { get; }

        public Location Location { get; }
    }
}
