using System;

namespace NeoServer.Game.Contracts.World.Tiles
{
    public interface ITileEnterRule
    {
        Func<ITile, bool> CanEnter { get; }
    }
}
