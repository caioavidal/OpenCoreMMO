using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Bases
{
    public class StaticItem : IItem
    {
        public StaticItem(IItemType metadata, Location location)
        {
            Metadata = metadata;
            Location = location;
        }

        public string CustomLookText => null; //todo: revise

        public IItemType Metadata { get; }

        public Location Location { get; set; }
    }
}