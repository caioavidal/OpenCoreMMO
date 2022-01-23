using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Items.Cumulatives;
using NeoServer.Game.Items.Items.UsableItems;

namespace NeoServer.Game.Items.Factories;

public class CumulativeFactory : IFactory
{
    public event CreateItem OnItemCreated;


    public IItem Create(IItemType itemType, Location location, IDictionary<ItemAttribute, IConvertible> attributes)
    {
        if (!ICumulative.IsApplicable(itemType)) return null;

        if (Coin.IsApplicable(itemType)) return new Coin(itemType, location, attributes);
        if (HealingItem.IsApplicable(itemType)) return new HealingItem(itemType, location, attributes);
        if (Food.IsApplicable(itemType)) return new Food(itemType, location, attributes);

        return new Cumulative(itemType, location, attributes);
    }
}