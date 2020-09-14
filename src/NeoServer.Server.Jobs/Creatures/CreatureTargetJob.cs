using NeoServer.Game.Contracts.Creatures;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Jobs.Creatures
{
    public class CreatureTargetJob
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

            if(monster.HasAnyTarget)
            {
                monster.SetAttackTarget();
            }
        }
    }
}
