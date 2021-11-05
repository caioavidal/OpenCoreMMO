using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Inspection;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items
{
    public struct TeleportItem : ITeleport, IItem
    {
        public TeleportItem(IItemType metadata, Location location,
            IDictionary<ItemAttribute, IConvertible> attributes)
        {
            Metadata = metadata;
            Location = location;

            Destination = Location.Zero;
                
            if (attributes is not null)
            {
                Destination = attributes.TryGetValue(ItemAttribute.TeleportDestination, out var destination) &&
                              destination is Location destLocation
                    ? destLocation
                    : Location.Zero;
            }
        }

        public Location Location { get; set; }

        public string GetLookText(IInspectionTextBuilder inspectionTextBuilder, bool isClose = false) =>
            inspectionTextBuilder is null
                ? $"You see {Metadata.Article} {Metadata.Name}"
                : inspectionTextBuilder.Build(this, isClose);

        public bool HasDestination => Destination != Location.Zero;
        public Location Destination { get; }

        public IItemType Metadata { get; }

        public bool Teleport(IWalkableCreature player)
        {
            if (!HasDestination) return false;
            player.TeleportTo(Destination);
            return true;
        }

        public static bool IsApplicable(IItemType type) => type
            .Attributes
            .GetAttribute(ItemAttribute.Type)
            ?.Equals("teleport", StringComparison.InvariantCultureIgnoreCase) ?? false;
    }
}