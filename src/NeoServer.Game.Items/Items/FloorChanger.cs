using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Items.Items
{
    public class FloorChanger: BaseItem, IUseable, IItem
    {
        
        public FloorChanger(IItemType metadata, Location location) : base(metadata)
        {
            Location = location;
        }
        public void Use(IPlayer player, IMap map)
        {
            if(Metadata.Attributes.GetAttribute(Common.ItemAttribute.FloorChange) == "up")
            {
                var toLocation = new Location(Location.X, Location.Y, (byte)(Location.Z - 1));
                foreach (var neighbour in toLocation.Neighbours)
                {
                    if(map[neighbour] is IDynamicTile tile)
                    {
                        map.TryMoveThing(player, tile.Location);
                        return;
                    }
                }
            }
        }
        public static bool IsApplicable(IItemType type) => type.Attributes.HasAttribute(Common.ItemAttribute.FloorChange) || type.HasFlag(Common.ItemFlag.Useable);

    
    }
}
