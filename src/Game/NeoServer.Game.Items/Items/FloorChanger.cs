using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Bases;

namespace NeoServer.Game.Items.Items
{
    public class FloorChanger : BaseItem, IUsable, IItem
    {
        public FloorChanger(IItemType metadata, Location location) : base(metadata)
        {
            Location = location;
        }

        public virtual void Use(IPlayer player)
        {
            if (!player.Location.IsNextTo(Location)) return;
            var toLocation = Location.Zero;

            var floorChange = Metadata.Attributes.GetAttribute(ItemAttribute.FloorChange);

            if (floorChange == "up") toLocation.Update(Location.X, Location.Y, (byte) (Location.Z - 1));
            if (floorChange == "down") toLocation.Update(Location.X, Location.Y, (byte) (Location.Z + 1));

            player.TeleportTo(toLocation);
        }

        public static bool IsApplicable(IItemType type)
        {
            return type.Attributes.HasAttribute(ItemAttribute.FloorChange) &&
                   type.HasFlag(ItemFlag.Useable);
        }
    }
}