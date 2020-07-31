using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Enums.Location.Structs;

namespace NeoServer.Game.Items.Items
{
    public readonly struct GroundItem : IGround
    {
        public ushort StepSpeed { get; }

        public Location Location { get; }

        public byte MovementPenalty { get; }

        public IItemType Metadata { get; }

        public GroundItem(IItemType type, Location location)
        {
            Metadata = type;
            StepSpeed = type?.Speed != 0 ? type.Speed : (ushort)150;
            Location = location;
            MovementPenalty = type.Attributes.GetAttribute<byte>(Enums.ItemAttribute.Waypoints);
        }
        public static bool IsApplicable(IItemType type) => type.Group == Enums.ItemGroup.Ground;

    }
}
