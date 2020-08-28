using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.World.Map;
using System;

namespace NeoServer.Server.Jobs.Creatures
{
    internal class CreatureFollowingJob
    {
        private const uint INTERVAL = 300;
        private static long _lastWalk = 0;

        public static void Execute(ICreature creature, Game game)
        {
            
            var now = DateTime.Now.Ticks;
            var remainingTime = TimeSpan.FromTicks(now - _lastWalk).TotalMilliseconds;

            if (remainingTime < INTERVAL)
            {
                //return;
            }

          
      

            if (creature.IsDead)
            {
                return;
            }
            if (creature.IsFollowing)
            {
                if (!game.CreatureManager.TryGetCreature(creature.Following, out ICreature enemy))
                {
                    return;
                }

                var p = new PathFinder();
                var directions = p.Find(creature.Location, enemy.Location);

                creature.TryWalkTo(directions);

                var thing = creature as IMoveableThing;

                if (creature.TryGetNextStep(out var direction))
                {
                    var toTile = game.Map.GetNextTile(thing.Location, direction);
                    if (!game.Map.TryMoveThing(ref thing, toTile.Location))
                    {
                        // player.CancelWalk();
                    }
                }
                _lastWalk = now;
            }

            

        }
    }
}