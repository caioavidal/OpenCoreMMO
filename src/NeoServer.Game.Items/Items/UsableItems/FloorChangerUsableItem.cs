using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Items.Items.UsableItems
{
    public class FloorChangerUsableItem : UseableOnItem
    {
        public FloorChangerUsableItem(IItemType type, Location location) : base(type, location) { }

        public override bool Use(ICreature usedBy, IItem item)
        {
            if (usedBy is not IPlayer player) return false;
            if (Metadata.OnUse?.GetAttribute<ushort>(Common.ItemAttribute.UseOn) != item.Metadata.TypeId) return false;

            if (Metadata.OnUse?.GetAttribute(Common.ItemAttribute.FloorChange) == "up")
            {
                var toLocation = new Location(item.Location.X, item.Location.Y, (byte)(item.Location.Z - 1));

                player.TeleportTo(toLocation);
                return true;
            }
            return false;
        }
        public static new bool IsApplicable(IItemType type) => UseableOnItem.IsApplicable(type) && (type.OnUse?.HasAttribute(Common.ItemAttribute.FloorChange) ?? false);

    }
}
