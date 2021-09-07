using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items
{
    public abstract class BaseItem : IItem
    {
        protected BaseItem(IItemType metadata)
        {
            Metadata = metadata;
        }

        public IItemType Metadata { get; protected set; }
        public Location Location { get; set; }
        public byte Amount { get; set; } = 1;
    }
}