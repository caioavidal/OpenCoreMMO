using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items.Containers;

public class Depot : Container.Container, IDepot
{
    public Depot(IItemType type, Location location, IEnumerable<IItem> children) : base(type, location, children)
    {
    }


    public override void ClosedBy(IPlayer player)
    {
        if (RootParent is not IDepot || player.HasDepotOpened) return;
        Clear();
        base.ClosedBy(player);
    }

    public new static bool IsApplicable(IItemType metadata)
    {
        if (metadata.Group is not ItemGroup.Container) return false;
        
        var type = metadata.Attributes.GetAttribute(ItemAttribute.Type);
        return type is not null && type.Equals("depot", StringComparison.InvariantCultureIgnoreCase);
    }
}