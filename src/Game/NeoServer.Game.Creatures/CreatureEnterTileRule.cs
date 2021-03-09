using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using System;

namespace NeoServer.Game.Creatures.Monsters
{
    public abstract class CreatureEnterTileRule<T> : ITileEnterRule
    {
        public virtual bool CanEnter(ITile tile, ICreature creature)
        {
            if (tile is not IDynamicTile dynamicTile) return false;

            return ConditionEvaluation.And(
                 dynamicTile.FloorDirection == Common.Location.FloorChangeDirection.None,
                 !dynamicTile.HasBlockPathFinding,
                 !dynamicTile.HasCreature,
                 dynamicTile.Ground is not null);
        }

        private static readonly Lazy<T> Lazy = new Lazy<T>(() => (T)Activator.CreateInstance(typeof(T), true));
        public static T Rule => Lazy.Value;

    }

    public class CreatureEnterTileRule : CreatureEnterTileRule<CreatureEnterTileRule>
    {
        public override bool CanEnter(ITile tile, ICreature creature)
        {
            if (tile is not IDynamicTile dynamicTile) return false;

            return ConditionEvaluation.And(
                 dynamicTile.FloorDirection == Common.Location.FloorChangeDirection.None,
                 !dynamicTile.HasBlockPathFinding,
                 !dynamicTile.HasCreature,
                 dynamicTile.Ground is not null);
        }
    }
    public class NpcEnterTileRule : CreatureEnterTileRule<NpcEnterTileRule>
    {
        public override bool CanEnter(ITile tile, ICreature creature)
        {
            if (creature is not INpc npc) return false;
            return base.CanEnter(tile, npc) && npc.SpawnPoint.Location.GetMaxSqmDistance(tile.Location) <= 3;
        }

    }
}