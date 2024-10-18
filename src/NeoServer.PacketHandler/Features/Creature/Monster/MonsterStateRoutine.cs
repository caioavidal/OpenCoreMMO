using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Creature.Monster.Managers;

namespace NeoServer.PacketHandler.Features.Creature.Monster;

public static class MonsterStateRoutine
{
    public static void Execute(IMonster monster, ISummonService summonService)
    {
        MonsterStateManager.Run(monster, summonService);
    }
}