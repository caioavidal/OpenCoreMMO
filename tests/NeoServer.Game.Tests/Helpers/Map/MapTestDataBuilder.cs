using System;
using System.Collections;
using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items;
using NeoServer.Game.Items.Items;
using NeoServer.Game.World.Map;
using NeoServer.Game.World.Models.Tiles;

namespace NeoServer.Game.Tests.Helpers
{
    public static class MapTestDataBuilder
    {
        
        public static Map Build(params ITile[] tiles)
        {
            var world = new World.World();

            foreach (var tile in tiles)
            {
                world.AddTile(tile);
            }
        
            return new Map(world);
        }
        public static Map Build(int fromX, int toX, int fromY, int toY, int fromZ, int toZ, bool addGround = false,
            IDictionary<Location, IItem[]> topItems = null,
            List<Location> staticTiles = null)
        {
            topItems ??= new Dictionary<Location, IItem[]>();
            staticTiles ??= new();
            
            var world = new World.World();

            for (var x = fromX; x <= toX; x++)
            for (var y = fromY; y <= toY; y++)
            for (var z = fromZ; z <= toZ; z++)
            {
                IGround ground = null;

                var location = new Location((ushort)x, (ushort)y, (byte)z);
                
                if (addGround)
                {
                    var itemType = new ItemType();
                    ground = new Ground(new ItemType(), new Location((ushort)x, (ushort)y, (byte)z));
                }

                topItems.TryGetValue(location, out var items);

                if (staticTiles.Contains(location))
                {
                    world.AddTile(new StaticTile(new Coordinate(x,y,(sbyte)z)));
                }

                world.AddTile(new DynamicTile(new Coordinate(x, y, (sbyte)z), TileFlag.None, ground , items ?? Array.Empty<IItem>(), null));
            }
            
            return new Map(world);
        }

        public static Ground CreateGround(Location location, int speed = 50)
        {
            var itemType = new ItemType();
            itemType.Attributes?.SetAttribute(ItemAttribute.Speed,speed);

            return new Ground(itemType, new Location(102, 100, 7));
        } 
    }
}