using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Items.Items.UsableItems
{
    public class FloorChangerUsableItem : UseableOnItem
    {
        public FloorChangerUsableItem(IItemType type, Location location) : base(type, location)
        {
        }

        public override void UseOn(IPlayer player, IMap map, IThing thing)
        {
            if (thing is not IItem item) return;

            if (Metadata.OnUse?.GetAttribute<ushort>(Common.ItemAttribute.UseOn) != item.Metadata.TypeId) return;

            if (Metadata.OnUse?.GetAttribute(Common.ItemAttribute.FloorChange) == "up")
            {
                var toLocation = new Location(thing.Location.X, thing.Location.Y, (byte)(thing.Location.Z - 1));
                foreach (var neighbour in toLocation.Neighbours)
                {
                    if (map[neighbour] is IDynamicTile tile)
                    {
                        map.TryMoveThing(player, tile.Location);
                        return;
                    }
                }
            }
            base.UseOn(player, map, thing);
        }
        public static new bool IsApplicable(IItemType type) => UseableOnItem.IsApplicable(type) && (type.OnUse?.HasAttribute(Common.ItemAttribute.FloorChange) ?? false);

    }
}
