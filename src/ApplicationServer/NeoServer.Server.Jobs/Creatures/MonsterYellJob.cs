using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Server.Jobs.Creatures;

public static class MonsterYellJob
{
    public static void Execute(IMonster monster)
    {
        if (monster.IsDead) return;

        monster.Yell();
    }
}