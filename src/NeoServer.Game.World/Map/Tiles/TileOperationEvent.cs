using NeoServer.Game.Common;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using System;

namespace NeoServer.Game.World.Map.Tiles
{
    public class TileOperationEvent
    {
        public static event Action<ITile, IThing, OperationResult<IThing>> OnTileChanged;

        public static void OnChanged(ITile tile, IThing thing, OperationResult<IThing> operation)
        {
            OnTileChanged?.Invoke(tile,thing, operation);
        }
    }
}
