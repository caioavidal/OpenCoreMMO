using NeoServer.Game.Contracts;
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
        private readonly CreatureStartedWalkingEventHandler _creatureStartedWalkingEventHandler;
        private readonly CreatureHealedEventHandler _creatureHealedEventHandler;
        private readonly CreatureChangedAttackTargetEventHandler _creatureChangedAttackTargetEventHandler;
        private readonly CreatureStartedFollowingEventHandler  _creatureStartedFollowingEventHandler;
        private readonly CreatureChangedSpeedEventHandler _creatureChangedSpeedEventHandler;
        private readonly IMap _map;
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
            IPlayerFactory playerFactory, IMonsterFactory monsterFactory, IMap map,
            CreatureStartedWalkingEventHandler creatureStartedWalkingEventHandler, CreatureHealedEventHandler creatureHealedEventHandler,
            CreatureChangedAttackTargetEventHandler creatureChangedAttackTargetEventHandler, CreatureStartedFollowingEventHandler creatureStartedFollowingEventHandler, 
            CreatureChangedSpeedEventHandler creatureChangedSpeedEventHandler)
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
            _map = map;
            _creatureStartedWalkingEventHandler = creatureStartedWalkingEventHandler;
            _creatureHealedEventHandler = creatureHealedEventHandler;
            _creatureChangedAttackTargetEventHandler = creatureChangedAttackTargetEventHandler;
            _creatureStartedFollowingEventHandler = creatureStartedFollowingEventHandler;
            _creatureChangedSpeedEventHandler = creatureChangedSpeedEventHandler;
        }
        public IMonster CreateMonster(string name, ISpawnPoint spawn = null)
        { 
            var monster = _monsterFactory.Create(name, spawn);
            AttachCombatActorEvents(monster);
            AttachWalkableEvents(monster);
            return AttachEvents(monster) as IMonster;

        }
        public IPlayer CreatePlayer(IPlayerModel playerModel)
        {
            var player = _playerFactory.Create(playerModel);
            AttachCombatActorEvents(player);
            AttachWalkableEvents(player);
            return AttachEvents(player) as IPlayer;
        }


        private ICreature AttachCombatActorEvents(ICombatActor actor)
        {
            actor.OnTargetChanged += _creatureChangedAttackTargetEventHandler.Execute;
            return actor;
        }
        private ICreature AttachWalkableEvents(IWalkableCreature creature)
        {
            creature.OnStartedFollowing += _creatureStartedFollowingEventHandler.Execute;
            creature.OnChangedSpeed += _creatureChangedSpeedEventHandler.Execute;
            return creature;
        }
        private ICreature AttachEvents(ICombatActor creature)
        {
            creature.OnDamaged += _creatureReceiveDamageEventHandler.Execute;
            creature.OnKilled += _creatureKilledEventHandler.Execute;
            //creature.OnWasBorn += _creatureWasBornEventHandler.Execute;
            creature.OnBlockedAttack += _creatureBlockedAttackEventHandler.Execute;
            creature.OnTurnedToDirection += _creatureTurnToDirectionEventHandler.Execute;
            creature.OnAttack += _map.PropagateAttack;
            creature.OnStartedWalking += _creatureStartedWalkingEventHandler.Execute;
            creature.OnHeal += _creatureHealedEventHandler.Execute;

            creature.OnKilled += DetachEvents;

            return creature;
        }
        private void DetachEvents(ICombatActor creature)
        {
            creature.OnDamaged -= _creatureReceiveDamageEventHandler.Execute;
            creature.OnKilled -= _creatureKilledEventHandler.Execute;
            creature.OnKilled -= DetachEvents;
            //creature.OnWasBorn += _creatureWasBornEventHandler.Execute;
            creature.OnBlockedAttack -= _creatureBlockedAttackEventHandler.Execute;
            creature.OnTurnedToDirection -= _creatureTurnToDirectionEventHandler.Execute;
            creature.OnAttack -= _map.PropagateAttack;
            creature.OnStartedWalking -= _creatureStartedWalkingEventHandler.Execute;
            creature.OnHeal -= _creatureHealedEventHandler.Execute;
        }
    }
}
