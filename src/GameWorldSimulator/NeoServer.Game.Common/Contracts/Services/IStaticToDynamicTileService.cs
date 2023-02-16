using NeoServer.Game.Common.Contracts.World.Tiles;

namespace NeoServer.Game.Common.Contracts.Services;

public interface IStaticToDynamicTileService
{
    ITile TransformIntoDynamicTile(ITile tile);
}