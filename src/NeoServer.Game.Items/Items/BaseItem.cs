using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Enums.Location.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Items.Items
{
    public abstract class BaseItem : IItem
    {
        public BaseItem(IItemType metadata)
        {
            Metadata = metadata;
        }

        public abstract Location Location { get; }

        public IItemType Metadata { get; }
    }
}
