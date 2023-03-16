using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Bases;

public class Item : BaseItem
{
    public Item(IItemType metadata, Location location) : base(metadata, location)
    {
    }
}