using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Creatures.Monster.Managers;

namespace NeoServer.Server.Jobs.Creatures;

public static class MonsterStateJob
{
    public static void Execute(IMonster monster, ISummonService summonService)
    {
        MonsterStateManager.Run(monster, summonService);
    }
}