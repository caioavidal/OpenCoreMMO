using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Common.Contracts.World.Tiles;

public interface ITileEnterRule
{
    bool CanEnter(ITile tile, ICreature creature);
}