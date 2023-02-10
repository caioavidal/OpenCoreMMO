using System;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Results;

namespace NeoServer.Game.World.Models.Tiles;

public class TileOperationEvent
{
    public static event Action<ITile, IItem, OperationResultList<IItem>> OnTileChanged;

    public static void OnChanged(ITile tile, IItem thing, OperationResultList<IItem> operation)
    {
        OnTileChanged?.Invoke(tile, thing, operation);
    }
}