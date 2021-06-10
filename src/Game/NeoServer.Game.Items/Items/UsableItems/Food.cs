using System;
using System.Collections.Generic;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Useables;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items.UsableItems
{
    public class Food : Cumulative, IConsumable, IFood
    {
        public Food(IItemType type, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(type,
            location, attributes)
        {
        }

        public event Use OnUsed;
        public int CooldownTime => 0;

        public void Use(IPlayer usedBy, ICreature creature)
        {
            if (creature is not IPlayer player) return;

            if (!player.Feed(this)) return;

            Reduce();

            OnUsed?.Invoke(usedBy, creature, this);
        }

        public static bool IsApplicable(IItemType type)
        {
            return type.Attributes.GetAttribute(ItemAttribute.Type) == "food" && ICumulative.IsApplicable(type);
        }
    }
}