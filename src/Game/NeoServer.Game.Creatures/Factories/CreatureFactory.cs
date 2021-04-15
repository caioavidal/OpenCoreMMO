using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Game.Creatures
{
    public class CreatureFactory : ICreatureFactory
    {
        //factories
        private readonly IMonsterFactory _monsterFactory;
        private readonly INpcFactory npcFactory;
        private readonly IEnumerable<ICreatureEventSubscriber> creatureEventSubscribers;
        public CreatureFactory(
           IMonsterFactory monsterFactory,
            IEnumerable<ICreatureEventSubscriber> creatureEventSubscribers, INpcFactory npcFactory)
        {

            _monsterFactory = monsterFactory;
            this.creatureEventSubscribers = creatureEventSubscribers;
            Instance = this;
            this.npcFactory = npcFactory;
        }
        public static ICreatureFactory Instance { get; private set; }
        public IMonster CreateMonster(string name, ISpawnPoint spawn = null)
        {
            var monster = _monsterFactory.Create(name, spawn);
            if (monster is null) return null;

            AttachEvents(monster);
            return monster;
        }
        public INpc CreateNpc(string name, ISpawnPoint spawn = null)
        {
            var npc = npcFactory.Create(name, spawn);
            if (npc is null) return null;

            AttachEvents(npc);
            return npc;

        }
        public IPlayer CreatePlayer(IPlayer player)
        {
            return AttachEvents(player) as IPlayer;
        }

        private ICreature AttachEvents(ICreature creature)
        {
            foreach (var gameSubscriber in creatureEventSubscribers.Where(x => x.GetType().IsAssignableTo(typeof(IGameEventSubscriber)))) //register game events first
            {
                gameSubscriber?.Subscribe(creature);
            }

            foreach (var subscriber in creatureEventSubscribers.Where(x => !x.GetType().IsAssignableTo(typeof(IGameEventSubscriber)))) //than register server events
            {
                subscriber?.Subscribe(creature);
            }

            return creature;
        }
    }
}
