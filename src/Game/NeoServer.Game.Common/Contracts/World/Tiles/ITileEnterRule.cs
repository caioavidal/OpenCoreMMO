using NeoServer.Game.Contracts.Creatures;
using System;

namespace NeoServer.Game.Contracts.World.Tiles
{
    public interface ITileEnterRule
    {
        bool CanEnter(ITile tile, ICreature creature);
    }
}
