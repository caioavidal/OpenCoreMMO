using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Items;

namespace NeoServer.Game.Items.Items
{
    public class Item : IItem
    {
        public Item(IItemType metadata, Location location)
        {
            Metadata = metadata;
            Location = location;
        }
        public IItemType Metadata { get; }

        public Location Location { get; set; }
    }
}
