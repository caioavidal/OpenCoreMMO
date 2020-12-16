using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using System;

namespace NeoServer.Game.World.Map.Tiles
{
    public class TileOperationEvent
    {
        public static event Action<ITile, ITileOperationResult> OnTileChanged;

        public static void OnChanged(ITile tile, ITileOperationResult operation)
        {
            OnTileChanged?.Invoke(tile, operation);
        }
    }
}
