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
        Clear();
        base.ClosedBy(player);
    }

    public new static bool IsApplicable(IItemType type) => type.Attributes.GetAttribute(ItemAttribute.Type) == "depot";
}