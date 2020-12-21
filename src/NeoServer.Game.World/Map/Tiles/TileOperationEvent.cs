using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using System;

namespace NeoServer.Game.World.Map.Tiles
{
    public class TileOperationEvent
    {
        public static event Action<ITile, IThing, IOperationResult> OnTileChanged;

        public static void OnChanged(ITile tile, IThing thing, IOperationResult operation)
        {
            OnTileChanged?.Invoke(tile,thing, operation);
        }
    }
}
