using System;
using System.Collections.Generic;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.Items.Types.Useables;

namespace NeoServer.Game.Items.Items.UsableItems
{
    public abstract class CumulativeUsableOnItem : Cumulative, IUseableOnItem
    {
        public CumulativeUsableOnItem(IItemType type, Location location,
            IDictionary<ItemAttribute, IConvertible> attributes) : base(type, location, attributes)
        {
        }

        public abstract bool Use(ICreature usedBy, IItem item);

        public static bool IsApplicable(IItemType type)
        {
            return UseableOnItem.IsApplicable(type) && ICumulative.IsApplicable(type);
        }
    }
}