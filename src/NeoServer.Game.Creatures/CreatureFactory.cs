using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Item;
using NeoServer.Server.Events;
using NeoServer.Server.Events.Combat;
using NeoServer.Server.Events.Creature;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Common;

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
        private readonly CreatureSayEventHandler _creatureSayEventHandler;
        private readonly CreatureChangedVisibilityEventHandler creatureTurnedInvisibleEventHandler;
        private readonly CreatureChangedOutfitEventHandler creatureChangedOutfitEventHandler;
        private ILiquidPoolFactory _liquidPoolFactory;

        private readonly IMap _map;
        //factories
        private readonly IPlayerFactory _playerFactory;
        private readonly IMonsterFactory _monsterFactory;
        private readonly IItemFactory itemFactory;
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
            CreatureChangedSpeedEventHandler creatureChangedSpeedEventHandler, CreatureSayEventHandler creatureSayEventHandler,
            ILiquidPoolFactory liquidPoolFactory, IItemFactory itemFactory, 
            CreatureChangedVisibilityEventHandler creatureTurnedInvisibleEventHandler, CreatureChangedOutfitEventHandler creatureChangedOutfitEventHandler)
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
            _creatureSayEventHandler = creatureSayEventHandler;
            _liquidPoolFactory = liquidPoolFactory;
            this.itemFactory = itemFactory;
            this.creatureTurnedInvisibleEventHandler = creatureTurnedInvisibleEventHandler;
            this.creatureChangedOutfitEventHandler = creatureChangedOutfitEventHandler;
        }
        public IMonster CreateMonster(string name, ISpawnPoint spawn = null)
        { 
            var monster = _monsterFactory.Create(name, spawn);
            if (monster is null) return null;

            AttachCombatActorEvents(monster);
            AttachWalkableEvents(monster);
            AttachEvents(monster);
            return monster;

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
        private ICreature AttachEvents(ICreature creature)
        {
            if (creature is ICombatActor combatActor)
            {
                combatActor.OnDamaged += _creatureReceiveDamageEventHandler.Execute;
                combatActor.OnKilled += _creatureKilledEventHandler.Execute;
                combatActor.OnKilled += AttachDeathEvent;
                combatActor.OnBlockedAttack += _creatureBlockedAttackEventHandler.Execute;
                combatActor.OnTurnedToDirection += _creatureTurnToDirectionEventHandler.Execute;
                combatActor.OnPropagateAttack += _map.PropagateAttack;
                combatActor.OnAttackEnemy += _creatureAttackEventHandler.Execute;
                combatActor.OnStartedWalking += _creatureStartedWalkingEventHandler.Execute;
                combatActor.OnHeal += _creatureHealedEventHandler.Execute;
                combatActor.OnKilled += DetachEvents;
                combatActor.OnDamaged += AttachDamageLiquidPoolEvent;
                combatActor.OnKilled += AttachDeathLiquidPoolEvent;
                combatActor.OnChangedVisibility += creatureTurnedInvisibleEventHandler.Execute;
            }

            creature.OnChangedOutfit += creatureChangedOutfitEventHandler.Execute;
            creature.OnSay += _creatureSayEventHandler.Execute;
            return creature;
        }
        private void DetachEvents(ICombatActor creature)
        {
            if (creature is IMonster monster && !monster.IsSummon) return;

            creature.OnDamaged -= _creatureReceiveDamageEventHandler.Execute;
            //creature.OnKilled -= _creatureKilledEventHandler.Execute;
            creature.OnKilled -= DetachEvents;
            creature.OnBlockedAttack -= _creatureBlockedAttackEventHandler.Execute;
            creature.OnTurnedToDirection -= _creatureTurnToDirectionEventHandler.Execute;
            creature.OnPropagateAttack -= _map.PropagateAttack;
            
            creature.OnAttackEnemy -= _creatureAttackEventHandler.Execute;
            creature.OnStartedWalking -= _creatureStartedWalkingEventHandler.Execute;
            creature.OnHeal -= _creatureHealedEventHandler.Execute;
            creature.OnSay -= _creatureSayEventHandler.Execute;
        }

        private void AttachDamageLiquidPoolEvent(ICombatActor enemy, ICombatActor victim, CombatDamage damage)
        {
            if (damage.IsElementalDamage) return;

            var liquidColor = victim.Blood switch
            {
                BloodType.Blood => LiquidColor.Red,
                BloodType.Slime => LiquidColor.Green
            };

            var pool = _liquidPoolFactory.CreateDamageLiquidPool(victim.Location, liquidColor);

            _map.CreateBloodPool(pool, victim.Tile);
        }
        private void AttachDeathLiquidPoolEvent(ICombatActor victim)
        {
            var liquidColor = victim.Blood switch
            {
                 BloodType.Blood => LiquidColor.Red,
                 BloodType.Slime => LiquidColor.Green
            };

            var pool = _liquidPoolFactory.Create(victim.Location, liquidColor);

            _map.CreateBloodPool(pool, victim.Tile);
        }

        private void AttachDeathEvent(ICreature creature)
        {
            var corpse = itemFactory.Create(creature.CorpseType, creature.Location, null);
            creature.Corpse = corpse;
            _map.ReplaceThing(creature, creature.Corpse);
        }
    }
}
