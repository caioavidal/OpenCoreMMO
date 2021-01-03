using NeoServer.Game.Contracts.World;

namespace NeoServer.Game.World.Map
{
    public struct Floor
    {
        public ITile[,] Tiles { get; set; }

        public ITile AddTile(ITile tile) => Tiles[tile.Location.X & 7, tile.Location.Y & 7] = tile;
    }
}
