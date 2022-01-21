using System;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World.Tiles;

namespace NeoServer.Game.World.Models.Tiles
{
    public class TileOperationEvent
    {
        public static event Action<ITile, IItem, OperationResult<IItem>> OnTileChanged;

        public static void OnChanged(ITile tile, IItem thing, OperationResult<IItem> operation)
        {
            OnTileChanged?.Invoke(tile, thing, operation);
        }
    }
}