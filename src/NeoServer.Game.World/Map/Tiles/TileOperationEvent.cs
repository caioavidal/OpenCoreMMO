using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using System;

namespace NeoServer.Game.World.Map.Tiles
{
    public class TileOperationEvent
    {
        public static event Action<ITile, IOperationResult> OnTileChanged;

        public static void OnChanged(ITile tile, IOperationResult operation)
        {
            OnTileChanged?.Invoke(tile, operation);
        }
    }
}
