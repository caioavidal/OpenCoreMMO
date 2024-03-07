using NeoServer.Application.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;

namespace NeoServer.Application.Features.Creature.Monster.Events;

public class MonsterChangedStateEventHandler : IEventHandler
{
    private readonly ICreatureGameInstance creatureGameInstance;

    public MonsterChangedStateEventHandler(ICreatureGameInstance creatureGameInstance)
    {
        this.creatureGameInstance = creatureGameInstance;
    }

    public void Execute(IMonster monster, MonsterState oldState, MonsterState toState)
    {
        if (toState == MonsterState.Sleeping)
            creatureGameInstance.TryRemove(monster.CreatureId);
        else if (oldState == MonsterState.Sleeping) creatureGameInstance.Add(monster);
    }
}