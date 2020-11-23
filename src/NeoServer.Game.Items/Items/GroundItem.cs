using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items
{
    public struct GroundItem : IGround
    {
        public ushort StepSpeed { get; }

        public byte MovementPenalty { get; }

        public IItemType Metadata { get; }
        public Location Location { get; set; }

        public GroundItem(IItemType type, Location location)
        {
            Metadata = type;
            StepSpeed = type?.Speed != 0 ? type.Speed : (ushort)150;
            Location = location;
            MovementPenalty = type.Attributes.GetAttribute<byte>(Common.ItemAttribute.Waypoints);
        }
        public static bool IsApplicable(IItemType type) => type.Group == Common.ItemGroup.Ground;

    }
}
