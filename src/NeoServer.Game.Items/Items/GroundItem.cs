using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Items;

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
            StepSpeed = type?.Speed != 0 ? type.Speed : 150;
            Location = location;
            MovementPenalty = type.Attributes.GetAttribute<byte>(Common.ItemAttribute.Waypoints);
        }
        public static bool IsApplicable(IItemType type) => type.Group == Common.ItemGroup.Ground;

    }
}
