using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Server.Events;
using NeoServer.Server.Events.Combat;
using NeoServer.Server.Events.Creature;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures
{
    public class CreatureFactory: ICreatureFactory
    {
        private readonly CreatureInjuredEventHandler _creatureReceiveDamageEventHandler;
        private readonly CreatureKilledEventHandler _creatureKilledEventHandler;
        private readonly CreatureWasBornEventHandler _creatureWasBornEventHandler;
        private readonly CreatureBlockedAttackEventHandler _creatureBlockedAttackEventHandler;
        private readonly CreatureAttackEventHandler _creatureAttackEventHandler;
        private readonly CreatureTurnedToDirectionEventHandler _creatureTurnToDirectionEventHandler;

        //factories
        private readonly IPlayerFactory _playerFactory;
        private readonly IMonsterFactory _monsterFactory;
        //private readonly IPathFinder _pathFinder;

        public CreatureFactory(
            CreatureInjuredEventHandler creatureReceiveDamageEventHandler, CreatureKilledEventHandler creatureKilledEventHandler,
            CreatureWasBornEventHandler creatureWasBornEventHandler, CreatureBlockedAttackEventHandler creatureBlockedAttackEventHandler,
            //IPathFinder pathFinder, 
            CreatureAttackEventHandler creatureAttackEventHandler,
            CreatureTurnedToDirectionEventHandler creatureTurnToDirectionEventHandler,
            IPlayerFactory playerFactory, IMonsterFactory monsterFactory)
        {
            _creatureReceiveDamageEventHandler = creatureReceiveDamageEventHandler;
            _creatureKilledEventHandler = creatureKilledEventHandler;
            _creatureWasBornEventHandler = creatureWasBornEventHandler;
            _creatureBlockedAttackEventHandler = creatureBlockedAttackEventHandler;
            //_pathFinder = pathFinder;
            _creatureAttackEventHandler = creatureAttackEventHandler;
            _creatureTurnToDirectionEventHandler = creatureTurnToDirectionEventHandler;
            _playerFactory = playerFactory;
            _monsterFactory = monsterFactory;
        }
        public IMonster CreateMonster(string name, ISpawnPoint spawn = null)
        { 
            var monster = _monsterFactory.Create(name, spawn);
            return AttachEvents(monster) as IMonster;

        }
        public IPlayer CreatePlayer(IPlayerModel playerModel)
        {
            var player = _playerFactory.Create(playerModel);
            return AttachEvents(player) as IPlayer;
        }


        private ICreature AttachEvents(ICreature creature)
        {
            creature.OnDamaged += _creatureReceiveDamageEventHandler.Execute;
            creature.OnKilled += _creatureKilledEventHandler.Execute;
            //creature.OnWasBorn += _creatureWasBornEventHandler.Execute;
            creature.OnBlockedAttack += _creatureBlockedAttackEventHandler.Execute;
            //creature.OnAttack += _creatureAttackEventHandler.Execute;
            creature.OnTurnedToDirection += _creatureTurnToDirectionEventHandler.Execute;
            return creature;
        }
    }
}
