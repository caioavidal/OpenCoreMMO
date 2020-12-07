using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Model.Bases;
using NeoServer.Server.Helpers;

namespace NeoServer.Game.Creatures.Monsters
{
    public abstract class WalkableMonster : CombatActor, IWalkableMonster
    {
        protected WalkableMonster(ICreatureType type, IPathAccess pathAccess, IOutfit outfit = null, uint healthPoints = 0) : base(type, pathAccess, outfit, healthPoints)
        {
        }
        public bool CanReachAnyTarget { get; protected set; } = false;

        public bool LookForNewEnemy()
        {
            StopFollowing();
            StopAttack();

            if (IsDead || CanReachAnyTarget) return false;

            int randomIndex = ServerRandom.Random.Next(minValue: 0, maxValue: 4);

            var directions = new Direction[4] { Direction.East, Direction.North, Direction.South, Direction.West };

            var direction = directions[randomIndex];

            if (PathAccess.CanGoToDirection?.Invoke(Location, direction, CreatureEnterTileRule.Rule) is false) return false;

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
    }

}
