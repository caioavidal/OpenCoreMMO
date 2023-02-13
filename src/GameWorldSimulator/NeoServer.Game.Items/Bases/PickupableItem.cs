using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Bases;

public class PickupableItem : MovableItem, IPickupable
{
    public PickupableItem(IItemType metadata, Location location) : base(metadata, location)
    {
    }
}