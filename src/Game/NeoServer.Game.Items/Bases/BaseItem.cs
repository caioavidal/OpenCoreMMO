using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Inspection;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Location.Structs;

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

        public string GetLookText(IInspectionTextBuilder inspectionTextBuilder, bool isClose = false) =>
            inspectionTextBuilder is null
                ? $"You see {Metadata.Article} {Metadata.Name}"
                : inspectionTextBuilder.Build(this, isClose);

        public byte Amount { get; set; } = 1;
        public void Transform(IPlayer @by) => OnTransform?.Invoke(@by, this, Metadata.Attributes.GetTransformationItem());
        public event Transform OnTransform;
    }
}