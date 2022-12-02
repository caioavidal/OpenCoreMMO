using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World.Tiles;

namespace NeoServer.Game.Common.Contracts.Services;

public interface IItemTransformerService
{
    IItem Transform(ITile tile, ushort fromItemId, ushort toItemId);
}