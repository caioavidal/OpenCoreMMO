﻿using System;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location;

namespace NeoServer.Game.Creatures
{
    public abstract class CreatureEnterTileRule<T> : ITileEnterRule
    {
        private static readonly Lazy<T> Lazy = new(() => (T) Activator.CreateInstance(typeof(T), true));
        public static T Rule => Lazy.Value;

        public virtual bool CanEnter(ITile tile, ICreature creature)
        {
            if (tile is not IDynamicTile dynamicTile) return false;

            return ConditionEvaluation.And(
                dynamicTile.FloorDirection == FloorChangeDirection.None,
                !dynamicTile.HasBlockPathFinding,
                !dynamicTile.HasCreature,
                dynamicTile.Ground is not null);
        }
    }

    public class CreatureEnterTileRule : CreatureEnterTileRule<CreatureEnterTileRule>
    {
        public override bool CanEnter(ITile tile, ICreature creature)
        {
            if (tile is not IDynamicTile dynamicTile) return false;

            return ConditionEvaluation.And(
                dynamicTile.FloorDirection == FloorChangeDirection.None,
                !dynamicTile.HasBlockPathFinding,
                !dynamicTile.HasCreature,
                dynamicTile.Ground is not null);
        }
    }

    public class MonsterEnterTileRule : CreatureEnterTileRule<MonsterEnterTileRule>
    {
        public override bool CanEnter(ITile tile, ICreature creature)
        {
            if (tile is not IDynamicTile dynamicTile) return false;
            if (creature is not IMonster monster) return false;

            return ConditionEvaluation.And(
                dynamicTile.FloorDirection == FloorChangeDirection.None,
                !dynamicTile.HasBlockPathFinding,
                !dynamicTile.HasCreature,
                !dynamicTile.ProtectionZone,
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