using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Items.Items;
using NeoServer.Game.Items.Tests;
using NeoServer.Game.World.Map.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace NeoServer.Game.World.Tests
{
    public class MapTest
    {
        [Fact]
        public void RemoveThing_RemovesThingFromTile()
        {
            var item = ItemTestData.CreateMoveableItem(2) as IItem;
            var world = new World();
            world.AddTile(new Tile(new Coordinate(100, 100, 7), TileFlag.None, new List<IItem>()
            {
                 ItemTestData.CreateRegularItem(1),
                 item
            }.ToArray())); 

            var map = new Map.Map(world);

            var thing = item as IMoveableThing;
            map.RemoveThing(ref thing, map[100, 100, 7] as IWalkableTile);

            Assert.Single((map[100, 100, 7] as IWalkableTile).DownItems);
            Assert.Equal(1, (map[100, 100, 7] as IWalkableTile).DownItems.First().ClientId);
        }
    }
}
