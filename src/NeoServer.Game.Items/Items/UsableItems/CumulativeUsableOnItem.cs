using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Items.Items.UsableItems
{
    public abstract class CumulativeUsableOnItem : Cumulative, IUseableOn
    {
        public CumulativeUsableOnItem(IItemType type, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(type, location, attributes)
        {
        }

        public static bool IsApplicable(IItemType type) => UseableOnItem.IsApplicable(type) && Cumulative.IsApplicable(type);

        public abstract void UseOn(IPlayer player, IMap map, IThing thing);
    }
}
