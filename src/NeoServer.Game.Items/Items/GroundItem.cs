using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Items.Items
{
    public readonly struct GroundItem : IGroundItem
    {
        public ushort StepSpeed { get; }

        public Location Location { get; }

        public IItemType Metadata { get; }

        public byte MovementPenalty {get;}

        public GroundItem(IItemType type, Location location)
        {
            MovementPenalty = type.Attributes.GetAttribute<byte>(Enums.ItemAttribute.Waypoints);
            Metadata = type;
            StepSpeed = type?.Speed != 0 ? type.Speed : (ushort)150;
            Location = location;
        }
        public static bool IsApplicable(IItemType type) => type.Group == Enums.ItemGroup.Ground;

    }
}
