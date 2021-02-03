using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Scripts.Events.Creatures;
using NeoServer.Scripts.Npcs.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Scripts.Events
{
    public class CreatureEventSubscriber : ICreatureEventSubscriber, IGameEventSubscriber
    {
        private readonly CreatureDroppedLootEventHandler creatureDroppedLootEventHandler;
        private readonly CreatureKilledEventHandler creatureKilledEventHandler;
        public CreatureEventSubscriber(CreatureDroppedLootEventHandler creatureDroppedLootEventHandler, CreatureKilledEventHandler creatureKilledEventHandler)
        {
            this.creatureDroppedLootEventHandler = creatureDroppedLootEventHandler;
            this.creatureKilledEventHandler = creatureKilledEventHandler;
        }

        public void Subscribe(ICreature creature)
        {
            if(creature is IMonster monster)
            {
                monster.OnDropLoot += creatureDroppedLootEventHandler.Execute;
            }
            if(creature is ICombatActor actor)
            {
                actor.OnKilled += creatureKilledEventHandler.Execute;
            }
            if(creature is INpc npc)
            {
                npc.OnAnswer += NpcActionHandler.OnAnswer;
            }
        }

        public void Unsubscribe(ICreature creature)
        {
            if (creature is IMonster monster)
            {
                monster.OnDropLoot -= creatureDroppedLootEventHandler.Execute;
            }
            if (creature is INpc npc)
            {
                npc.OnAnswer -= NpcActionHandler.OnAnswer;
            }
        }
    }
}
