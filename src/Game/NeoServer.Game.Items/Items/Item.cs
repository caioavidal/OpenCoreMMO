using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items
{
    public class Item : IItem
    {
        public Item(IItemType metadata, Location location)
        {
            Metadata = metadata;
            Location = location;
        }

        public string CustomLookText => null; //todo: revise

        public IItemType Metadata { get; }

        public Location Location { get; set; }
    }
}