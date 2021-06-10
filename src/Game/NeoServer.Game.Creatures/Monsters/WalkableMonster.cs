using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Model.Bases;

namespace NeoServer.Game.Creatures.Monsters
{
    public abstract class WalkableMonster : CombatActor, IWalkableMonster
    {
        protected WalkableMonster(ICreatureType type, IOutfit outfit = null, uint healthPoints = 0) : base(type, outfit,
            healthPoints)
        {
        }

        public bool CanReachAnyTarget { get; protected set; } = false;
        public override ITileEnterRule TileEnterRule => MonsterEnterTileRule.Rule;

        public bool LookForNewEnemy()
        {
            StopFollowing();
            StopAttack();

            if (IsDead || CanReachAnyTarget) return false;

            var direction = GetRandomStep();

            if (direction == Direction.None) return false;

            TryWalkTo(direction);

            return true;
        }

        public void Escape(Location fromLocation)
        {
            StopFollowing();

            if (IsDead) return;
            if (PathFinder is null) return;
            if (PathFinder.Find(this, fromLocation, FindPathParams.EscapeParams, TileEnterRule,
                out var directions) is false) return;

            TryWalkTo(directions);
        }

        public void MoveAroundEnemy(Location targetLocation)
        {
            if (!Attacking) return;

            if (!Cooldowns.Expired(CooldownType.MoveAroundEnemy)) return;
            Cooldowns.Start(CooldownType.MoveAroundEnemy, GameRandom.Random.Next(3000, maxValue: 5000));

            var direction = GetRandomStep();
            if (direction == Direction.None) return;

            var nextLocation = Location.GetNextLocation(direction);

            if (targetLocation.GetMaxSqmDistance(nextLocation) > PathSearchParams.MaxTargetDist) return;

            TryWalkTo(direction);
        }
    }
}