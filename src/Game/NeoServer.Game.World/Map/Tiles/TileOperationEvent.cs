using NeoServer.Game.Common;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using System;

namespace NeoServer.Game.World.Map.Tiles
{
    public class TileOperationEvent
    {
        public static event Action<ITile, IItem, OperationResult<IItem>> OnTileChanged;


        public static void OnChanged(ITile tile, IItem thing, OperationResult<IItem> operation)
        {
            OnTileChanged?.Invoke(tile,thing, operation);
        }
     
    }
}
