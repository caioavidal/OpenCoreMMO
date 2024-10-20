using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Modules.Combat.Events;
using NeoServer.Modules.Creatures.Events;
using NeoServer.Modules.Creatures.Monster.Events;

namespace NeoServer.Modules.Creatures.Monster;

public class MonsterEventSubscriber : ICreatureEventSubscriber, IServerEventSubscriber
{
    private readonly CreatureAttackEventHandler _creatureAttackEventHandler;

    private readonly CreatureWasBornEventHandler _creatureWasBornEventHandler;
    private readonly MonsterChangedStateEventHandler monsterChangedStateEventHandler;

    public MonsterEventSubscriber(CreatureWasBornEventHandler creatureWasBornEventHandler,
        CreatureAttackEventHandler creatureAttackEventHandler,
        MonsterChangedStateEventHandler monsterChangedStateEventHandler)
    {
        _creatureWasBornEventHandler = creatureWasBornEventHandler;
        _creatureAttackEventHandler = creatureAttackEventHandler;
        this.monsterChangedStateEventHandler = monsterChangedStateEventHandler;
    }

    public void Subscribe(ICreature creature)
    {
        if (creature is not IMonster monster) return;

        monster.OnWasBorn += _creatureWasBornEventHandler.Execute;
        monster.OnAttackEnemy += _creatureAttackEventHandler.Execute;
        monster.OnChangedState += monsterChangedStateEventHandler.Execute;
    }

    public void Unsubscribe(ICreature creature)
    {
        if (creature is not IMonster monster) return;

        monster.OnWasBorn -= _creatureWasBornEventHandler.Execute;
        monster.OnAttackEnemy -= _creatureAttackEventHandler.Execute;
        monster.OnChangedState -= monsterChangedStateEventHandler.Execute;
    }
}