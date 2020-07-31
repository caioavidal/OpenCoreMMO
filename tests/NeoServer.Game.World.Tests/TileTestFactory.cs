using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Items.Tests;
using NeoServer.Game.World.Map.Tiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.World.Tests
{
    public class TileTestFactory
    {
        private static ITile CreateTile(Coordinate coord, params IItem[] item)
        {
            var items = new List<IItem> {
                ItemTestData.CreateTopItem(id: 1, topOrder: 1),
                ItemTestData.CreateTopItem(id: 2, topOrder: 2),
                ItemTestData.CreateRegularItem(100),
                ItemTestData.CreateRegularItem(200)
            };
            items.AddRange(item);

            var tile = new Tile(coord, TileFlag.None, items.ToArray());
            return tile;
        }
    }
}
