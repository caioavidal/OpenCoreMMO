using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Enums.Location.Structs;

namespace NeoServer.Game.Items.Items
{
    public struct Item : IItem
    {
        public Item(IItemType metadata, Location location)
        {
            Metadata = metadata;
            Location = location;
        }
        public IItemType Metadata { get; }

        public Location Location { get; }
    }
}
