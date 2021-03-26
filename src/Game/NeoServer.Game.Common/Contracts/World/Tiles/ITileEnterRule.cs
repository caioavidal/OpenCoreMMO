using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Contracts.World.Tiles
{
    public interface ITileEnterRule
    {
        bool CanEnter(ITile tile, ICreature creature);
    }
}
