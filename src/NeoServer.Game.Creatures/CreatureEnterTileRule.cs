using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using System;

namespace NeoServer.Game.Creatures.Monsters
{
    public class CreatureEnterTileRule : ITileEnterRule
    {
        public Func<ITile, bool> CanEnter => (tile) => tile is IDynamicTile dynamicTile && dynamicTile.FloorDirection == Common.Location.FloorChangeDirection.None 
        && !dynamicTile.HasBlockPathFinding && !dynamicTile.HasCreature && dynamicTile.Ground is not null;

        private static CreatureEnterTileRule rule;
        public static CreatureEnterTileRule Rule
        {
            get
            {
                rule = rule ?? new CreatureEnterTileRule();
                return rule;
            }
        }

    } 
}