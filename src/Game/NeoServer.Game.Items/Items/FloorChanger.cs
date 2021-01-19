using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Items;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Items.Items
{
    public class FloorChanger : BaseItem, IUseable, IItem
    {
        public FloorChanger(IItemType metadata, Location location) : base(metadata)
        {
            Location = location;
        }
        public void Use(IPlayer player)
        {
            if (!player.Location.IsNextTo(Location)) return;
            Location toLocation = Location.Zero;

            var floorChange = Metadata.Attributes.GetAttribute(Common.ItemAttribute.FloorChange);

            if (floorChange == "up") toLocation.Update(Location.X, Location.Y, (byte)(Location.Z - 1));
            if (floorChange == "down") toLocation.Update(Location.X, Location.Y, (byte)(Location.Z + 1));

            player.TeleportTo(toLocation);
            return;

        }
        public static bool IsApplicable(IItemType type) => type.Attributes.HasAttribute(Common.ItemAttribute.FloorChange) && type.HasFlag(Common.ItemFlag.Useable);
    }
}