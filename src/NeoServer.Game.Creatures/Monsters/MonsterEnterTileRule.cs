using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using System;

namespace NeoServer.Game.Creatures.Monsters
{
    public class MonsterEnterTileRule : ITileEnterRule
    {
        public Func<ITile, bool> CanEnter => (tile) => tile is IDynamicTile dynamicTile && dynamicTile.FloorDirection == Common.Location.FloorChangeDirection.None && !dynamicTile.HasBlockPathFinding && !dynamicTile.HasCreature;
    } 
}