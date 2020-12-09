using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Server.Helpers;
using NeoServer.Server.Tasks;
using System;

namespace NeoServer.Server.Jobs.Creatures
{
    public class MonsterYellJob
    {        
        public static void Execute(IMonster monster)
        {
            if (monster.IsDead) return;
            
            monster.Yell();
        }
    }
}
