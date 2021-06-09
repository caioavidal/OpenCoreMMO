using System;
using System.Collections.Generic;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;

namespace NeoServer.Game.Items.Items
{
    public class Coin : Cumulative, ICoin
    {
        public Coin(IItemType type, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(type,
            location, attributes)
        {
        }

        public Coin(IItemType type, Location location, byte amount) : base(type, location, amount)
        {
        }

        public uint WorthMultiplier => Metadata.Attributes.GetAttribute<uint>(ItemAttribute.Worth);
        public uint Worth => Amount * WorthMultiplier;

        public static bool IsApplicable(IItemType type)
        {
            return ICumulative.IsApplicable(type) && (type.Attributes.GetAttribute(ItemAttribute.Type)
                ?.Equals("coin", StringComparison.InvariantCultureIgnoreCase) ?? false);
        }
    }
}