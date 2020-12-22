using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items
{
    public abstract class BaseItem : IItem
    {
        public BaseItem(IItemType metadata)
        {
            Metadata = metadata;
        }
        public IItemType Metadata { get; }
        public Location Location { get; set; }
        public byte Amount { get; set; } = 1;
    }
}
