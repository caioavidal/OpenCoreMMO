using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Creatures.Model.Bases;

namespace NeoServer.Game.Creatures.Monsters
{
    public abstract class WalkableMonster : CombatActor, IWalkableMonster
    {
        protected WalkableMonster(ICreatureType type, IPathAccess pathAccess, IOutfit outfit = null, uint healthPoints = 0) : base(type, pathAccess, outfit, healthPoints) { }
        public bool CanReachAnyTarget { get; protected set; } = false;

        private Direction GetRandomStep()
        {
            int randomIndex = GameRandom.Random.Next(minValue: 0, maxValue: 4);

            var directions = new Direction[4] { Direction.East, Direction.North, Direction.South, Direction.West };

            var direction = directions[randomIndex];

            if (PathAccess.CanGoToDirection?.Invoke(Location, direction, CreatureEnterTileRule.Rule) is false) return Direction.None;

            return direction;
        }
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
            if (PathAccess.FindPathToDestination is null) return;
            if (PathAccess.FindPathToDestination.Invoke(this, fromLocation, FindPathParams.EscapeParams, CreatureEnterTileRule.Rule, out var directions) is false) return;

            TryWalkTo(directions);
        }

        public void MoveAroundEnemy(Location targetLocation)
        {
            if (!Attacking) return;

            if (!Cooldowns.Expired(CooldownType.MoveAroundEnemy)) return;
            Cooldowns.Start(CooldownType.MoveAroundEnemy, GameRandom.Random.Next(minValue: 3000, maxValue: 5000));

            var direction = GetRandomStep();
            if (direction == Direction.None) return;

            var nextLocation = Location.GetNextLocation(direction);

            if (targetLocation.GetMaxSqmDistance(nextLocation) > PathSearchParams.MaxTargetDist) return;

            TryWalkTo(direction);
        }
    }
}