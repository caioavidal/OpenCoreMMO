using NeoServer.Game.Contracts.World;
using NeoServer.Game.World.Map.Tiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.World.Map
{
    public struct Floor
    {
        public ITile[,] Tiles { get; set; }

        public ITile AddTile(ITile tile) => Tiles[tile.Location.X & 7, tile.Location.Y & 7] = tile;
    }
}
