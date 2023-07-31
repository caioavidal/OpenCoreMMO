using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Items.UsableItems.Runes;

namespace NeoServer.Game.Items.Factories;

public class RuneFactory : IFactory
{
    public event CreateItem OnItemCreated;

    public IItem Create(IItemType itemType, Location location, IDictionary<ItemAttribute, IConvertible> attributes)
    {
        if (!ICumulative.IsApplicable(itemType)) return null;
        if (!Rune.IsApplicable(itemType)) return null;

        if (AttackRune.IsApplicable(itemType)) return new AttackRune(itemType, location, attributes);
        if (FieldRune.IsApplicable(itemType)) return new FieldRune(itemType, location, attributes);
        
        return new SupportRune(itemType, location, attributes);
    }
}