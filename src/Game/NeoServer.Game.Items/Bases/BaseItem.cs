using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Inspection;

namespace NeoServer.Game.Items.Bases
{
    public abstract class BaseItem : IItem
    {
        protected BaseItem(IItemType metadata)
        {
            Metadata = metadata;
        }

        public IItemType Metadata { get; protected set; }
        public Location Location { get; set; }
        public string GetLookText(bool isClose = false) => this.Build(isClose);

        public byte Amount { get; set; } = 1;
    }
}