using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items.Containers;

public class Depot : Container, IDepot
{
    public Depot(IItemType type, Location location) : base(type, location)
    {
    }


    public override void ClosedBy(IPlayer player)
    {
        Clear();
        base.ClosedBy(player);
    }

    public new static bool IsApplicable(IItemType type)
    {
        return type.Attributes.GetAttribute(ItemAttribute.Type) == "depot";
    }
}