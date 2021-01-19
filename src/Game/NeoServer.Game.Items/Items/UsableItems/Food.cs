using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Items.Items.UsableItems
{
    public class Food : Cumulative, IConsumable, IFood
    {
        public Food(IItemType type, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(type, location, attributes)
        {
        }

        public event Use OnUsed;

        public void Use(IPlayer usedBy, ICreature creature)
        {
            if (creature is not IPlayer player) return;

            if (!player.Feed(this)) return;

            Reduce(1);

            OnUsed?.Invoke(usedBy, creature, this);
        }
        public static new bool IsApplicable(IItemType type) => type.Attributes.GetAttribute(ItemAttribute.Type) == "food" && Cumulative.IsApplicable(type);

    }
}
