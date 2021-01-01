using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Server.Model.Players.Contracts;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Server.Model.Players;

namespace NeoServer.Game.Creatures
{
    public class CreatureFactory : ICreatureFactory
    {
        //factories
        private readonly IPlayerFactory _playerFactory;
        private readonly IMonsterFactory _monsterFactory;
        private readonly IEnumerable<ICreatureEventSubscriber> creatureEventSubscribers;
        public CreatureFactory(
            IPlayerFactory playerFactory, IMonsterFactory monsterFactory,
            IEnumerable<ICreatureEventSubscriber> creatureEventSubscribers)
        {
            _playerFactory = playerFactory;
            _monsterFactory = monsterFactory;

            this.creatureEventSubscribers = creatureEventSubscribers;
        }
        public IMonster CreateMonster(string name, ISpawnPoint spawn = null)
        {
            var monster = _monsterFactory.Create(name, spawn);
            if (monster is null) return null;

            AttachEvents(monster);
            return monster;
        }
        public IPlayer CreatePlayer(IPlayer playerModel)
        {
            var player = _playerFactory.Create(playerModel);
            return AttachEvents(player) as IPlayer;
            //return playerModel;
        }

        private ICreature AttachEvents(ICreature creature)
        {
            foreach (var gameSubscriber in creatureEventSubscribers.Where(x => x.GetType().IsAssignableTo(typeof(IGameEventSubscriber)))) //register game events first
            {
                gameSubscriber.Subscribe(creature);
            }

            foreach (var subscriber in creatureEventSubscribers.Where(x => !x.GetType().IsAssignableTo(typeof(IGameEventSubscriber)))) //than register server events
            {
                subscriber.Subscribe(creature);
            }

            return creature;
        }
    }
}
