using NeoServer.Game.Contracts.Creatures;

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
