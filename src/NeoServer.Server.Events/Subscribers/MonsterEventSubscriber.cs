using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Events.Combat;
using NeoServer.Server.Events.Creature;
using NeoServer.Server.Events.Creature.Monsters;

namespace NeoServer.Server.Events
{
    public class MonsterEventSubscriber : ICreatureEventSubscriber
    {

        private readonly CreatureWasBornEventHandler _creatureWasBornEventHandler;
        private readonly CreatureAttackEventHandler _creatureAttackEventHandler;
        private readonly CreatureDroppedLootEventHandler creatureDroppedLootEventHandler;
        private readonly MonsterChangedStateEventHandler monsterChangedStateEventHandler;

        public MonsterEventSubscriber(CreatureWasBornEventHandler creatureWasBornEventHandler, CreatureAttackEventHandler creatureAttackEventHandler, 
            CreatureDroppedLootEventHandler creatureDroppedLootEventHandler, 
            MonsterChangedStateEventHandler monsterChangedStateEventHandler)
        {
            _creatureWasBornEventHandler = creatureWasBornEventHandler;
            _creatureAttackEventHandler = creatureAttackEventHandler;
            this.creatureDroppedLootEventHandler = creatureDroppedLootEventHandler;
            this.monsterChangedStateEventHandler = monsterChangedStateEventHandler;
        }

        public void Subscribe(ICreature creature)
        {
            if (creature is not IMonster monster) return;

            monster.OnWasBorn += _creatureWasBornEventHandler.Execute;
            monster.OnAttackEnemy += _creatureAttackEventHandler.Execute;
            monster.OnDropLoot += creatureDroppedLootEventHandler.Execute;
            monster.OnChangedState += monsterChangedStateEventHandler.Execute;
        }

        public void Unsubscribe(ICreature creature)
        {
            if (creature is not IMonster monster) return;

            monster.OnWasBorn -= _creatureWasBornEventHandler.Execute;
            monster.OnAttackEnemy -= _creatureAttackEventHandler.Execute;
            monster.OnDropLoot -= creatureDroppedLootEventHandler.Execute;
            monster.OnChangedState -= monsterChangedStateEventHandler.Execute;
        }
    }
}
