using System;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Results;

namespace NeoServer.Game.World.Models.Tiles;

public static class TileOperationEvent
{
    public static event Action<ITile, IItem, OperationResultList<IItem>> OnTileChanged;
    public static event Action<ITile> OnTileLoaded;

    public static void OnChanged(ITile tile, IItem thing, OperationResultList<IItem> operation)
    {
        OnTileChanged?.Invoke(tile, thing, operation);
    }

    public static void OnLoaded(ITile tile)
    {
        OnTileLoaded?.Invoke(tile);
    }
}