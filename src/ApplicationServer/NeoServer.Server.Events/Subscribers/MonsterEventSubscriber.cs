using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Server.Events.Combat;
using NeoServer.Server.Events.Creature;
using NeoServer.Server.Events.Creature.Monsters;

namespace NeoServer.Server.Events.Subscribers;

public class MonsterEventSubscriber : ICreatureEventSubscriber
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