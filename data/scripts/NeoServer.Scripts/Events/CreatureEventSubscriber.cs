using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Scripts.Events.Creatures;
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

        public CreatureEventSubscriber(CreatureDroppedLootEventHandler creatureDroppedLootEventHandler)
        {
            this.creatureDroppedLootEventHandler = creatureDroppedLootEventHandler;
        }

        public void Subscribe(ICreature creature)
        {
            if(creature is IMonster monster)
            {
                monster.OnDropLoot += creatureDroppedLootEventHandler.Execute;
            }
        }

        public void Unsubscribe(ICreature creature)
        {
            if (creature is IMonster monster)
            {
                monster.OnDropLoot -= creatureDroppedLootEventHandler.Execute;
            }
        }
    }
}
