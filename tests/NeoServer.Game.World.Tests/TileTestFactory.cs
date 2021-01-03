using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Items.Tests;
using NeoServer.Game.World.Map.Tiles;
using System.Collections.Generic;

namespace NeoServer.Game.World.Tests
{
    public class TileTestFactory
    {
        private static ITile CreateTile(Coordinate coord, params IItem[] item)
        {
            var topItems = new List<IItem>()
            {
                ItemTestData.CreateTopItem(id: 1, topOrder: 1),
                ItemTestData.CreateTopItem(id: 2, topOrder: 2)
            };

            var items = new List<IItem> {

                ItemTestData.CreateRegularItem(100),
                ItemTestData.CreateRegularItem(200)
            };
            items.AddRange(item);

            var tile = new Tile(coord, TileFlag.None, null, topItems.ToArray(), items.ToArray());
            return tile;
        }
    }
}
