using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Runes;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items.UsableItems.Runes;

public class SupportRune : Rune,ISupportRune
{
    public SupportRune(IItemType type, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(
        type, location, attributes)
    {
    }

    public SupportRune(IItemType type, Location location, byte amount) : base(type, location, amount)
    {
    }

    public override ushort Duration { get; }
}