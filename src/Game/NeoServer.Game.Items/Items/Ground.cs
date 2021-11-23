using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Inspection;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items
{
    public class Ground: IGround
    {
        public ushort StepSpeed { get; }
        public byte MovementPenalty { get; }

        public IItemType Metadata { get; }
        public Location Location { get; set; }
        public string GetLookText(IInspectionTextBuilder inspectionTextBuilder, bool isClose = false) =>
            inspectionTextBuilder is null
                ? $"You see {Metadata.Article} {Metadata.Name}."
                : inspectionTextBuilder.Build(this, isClose);
        
        public Ground(IItemType type, Location location)
        {
            Metadata = type;
            StepSpeed = type?.Speed != 0 ? type.Speed : (ushort) 150;
            Location = location;
            MovementPenalty = type.Attributes.GetAttribute<byte>(ItemAttribute.Waypoints);
            OnTransform = default;
        }

        public static bool IsApplicable(IItemType type)
        {
            return type.Group == ItemGroup.Ground;
        }

        public void Transform(IPlayer @by) => OnTransform?.Invoke(@by, this, Metadata.Attributes.GetTransformationItem());
        public event Transform OnTransform;
    }
}