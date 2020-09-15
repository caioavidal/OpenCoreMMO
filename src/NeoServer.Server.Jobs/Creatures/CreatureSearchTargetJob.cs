using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.World.Map.Tiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Jobs.Creatures
{
    public class CreatureMovementJob
    {
        private const uint INTERVAL = 1000;
        public static void Execute(ICreature creature, Game game)
        {
            if (creature.IsDead)
            {
                return;
            }
            if (!(creature is IMonster monster))
            {
                return;
            }

            if(creature.TryGetNextStep(out var direction))
            {
                var thing = creature as IMoveableThing;

                var toTile = game.Map.GetNextTile(thing.Location, direction);

                if(toTile is IImmutableTile)
                {
                    return;
                }

                if(toTile is IWalkableTile walkableTile && walkableTile.HasBlockPathFinding)
                {
                    return;
                }
                     
                game.Map.TryMoveThing(ref thing, toTile.Location);
            }
        }
    }
}
