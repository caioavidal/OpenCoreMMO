using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Events.Combat;
using NeoServer.Server.Events.Creature;

namespace NeoServer.Server.Events
{
    public class CreatureEventSubscriber : ICreatureEventSubscriber
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
        private readonly CreatureStartedFollowingEventHandler _creatureStartedFollowingEventHandler;
        private readonly CreatureChangedSpeedEventHandler _creatureChangedSpeedEventHandler;
        private readonly CreatureHearEventHandler creatureHearEventHandler;
        private readonly CreatureChangedVisibilityEventHandler creatureTurnedInvisibleEventHandler;
        private readonly CreatureChangedOutfitEventHandler creatureChangedOutfitEventHandler;
        private readonly PlayerSentMessageEventHandler creatureSentMessageEventHandler;
        public CreatureEventSubscriber(CreatureInjuredEventHandler creatureReceiveDamageEventHandler, CreatureKilledEventHandler creatureKilledEventHandler,
            CreatureWasBornEventHandler creatureWasBornEventHandler, CreatureBlockedAttackEventHandler creatureBlockedAttackEventHandler, CreatureAttackEventHandler creatureAttackEventHandler,
            CreatureTurnedToDirectionEventHandler creatureTurnToDirectionEventHandler, CreatureStartedWalkingEventHandler creatureStartedWalkingEventHandler,
            CreatureHealedEventHandler creatureHealedEventHandler, CreatureChangedAttackTargetEventHandler creatureChangedAttackTargetEventHandler,
            CreatureStartedFollowingEventHandler creatureStartedFollowingEventHandler, CreatureChangedSpeedEventHandler creatureChangedSpeedEventHandler,
            CreatureHearEventHandler creatureHearEventHandler,
            CreatureChangedVisibilityEventHandler creatureTurnedInvisibleEventHandler,
            CreatureChangedOutfitEventHandler creatureChangedOutfitEventHandler, PlayerSentMessageEventHandler creatureSentMessageEventHandler)
        {
            _creatureReceiveDamageEventHandler = creatureReceiveDamageEventHandler;
            _creatureKilledEventHandler = creatureKilledEventHandler;
            _creatureWasBornEventHandler = creatureWasBornEventHandler;
            _creatureBlockedAttackEventHandler = creatureBlockedAttackEventHandler;
            _creatureAttackEventHandler = creatureAttackEventHandler;
            _creatureTurnToDirectionEventHandler = creatureTurnToDirectionEventHandler;
            _creatureStartedWalkingEventHandler = creatureStartedWalkingEventHandler;
            _creatureHealedEventHandler = creatureHealedEventHandler;
            _creatureChangedAttackTargetEventHandler = creatureChangedAttackTargetEventHandler;
            _creatureStartedFollowingEventHandler = creatureStartedFollowingEventHandler;
            _creatureChangedSpeedEventHandler = creatureChangedSpeedEventHandler;
            this.creatureHearEventHandler = creatureHearEventHandler;
            this.creatureTurnedInvisibleEventHandler = creatureTurnedInvisibleEventHandler;
            this.creatureChangedOutfitEventHandler = creatureChangedOutfitEventHandler;
            this.creatureSentMessageEventHandler = creatureSentMessageEventHandler;
        }

        public void Subscribe(ICreature creature)
        {
            creature.OnChangedOutfit += creatureChangedOutfitEventHandler.Execute;
            

            if (creature is ISociableCreature sociableCreature)
            {
                sociableCreature.OnHear += creatureHearEventHandler.Execute;
            }
            if (creature is ICombatActor combatActor)
            {
                combatActor.OnTargetChanged += _creatureChangedAttackTargetEventHandler.Execute;
                combatActor.OnDamaged += _creatureReceiveDamageEventHandler.Execute;
                combatActor.OnKilled += _creatureKilledEventHandler.Execute;
                combatActor.OnBlockedAttack += _creatureBlockedAttackEventHandler.Execute;
                combatActor.OnTurnedToDirection += _creatureTurnToDirectionEventHandler.Execute;
                combatActor.OnAttackEnemy += _creatureAttackEventHandler.Execute;
                combatActor.OnStartedWalking += _creatureStartedWalkingEventHandler.Execute;
                combatActor.OnHeal += _creatureHealedEventHandler.Execute;
                combatActor.OnChangedVisibility += creatureTurnedInvisibleEventHandler.Execute;
            }

            #region WalkableEvents
            if (creature is IWalkableCreature walkableCreature)
            {
                walkableCreature.OnStartedFollowing += _creatureStartedFollowingEventHandler.Execute;
                walkableCreature.OnChangedSpeed += _creatureChangedSpeedEventHandler.Execute;
            }
            #endregion
        }

        public void Unsubscribe(ICreature creature)
        {
            creature.OnChangedOutfit -= creatureChangedOutfitEventHandler.Execute;

            if (creature is ICombatActor combatActor)
            {
                combatActor.OnTargetChanged -= _creatureChangedAttackTargetEventHandler.Execute;
                combatActor.OnDamaged -= _creatureReceiveDamageEventHandler.Execute;
                combatActor.OnKilled -= _creatureKilledEventHandler.Execute;
                combatActor.OnBlockedAttack -= _creatureBlockedAttackEventHandler.Execute;
                combatActor.OnTurnedToDirection -= _creatureTurnToDirectionEventHandler.Execute;
                combatActor.OnAttackEnemy -= _creatureAttackEventHandler.Execute;
                combatActor.OnStartedWalking -= _creatureStartedWalkingEventHandler.Execute;
                combatActor.OnHeal -= _creatureHealedEventHandler.Execute;
                combatActor.OnChangedVisibility -= creatureTurnedInvisibleEventHandler.Execute;
            }


            if (creature is IWalkableCreature walkableCreature)
            {
                walkableCreature.OnStartedFollowing -= _creatureStartedFollowingEventHandler.Execute;
                walkableCreature.OnChangedSpeed -= _creatureChangedSpeedEventHandler.Execute;
            }
            if (creature is ISociableCreature sociableCreature)
            {
                sociableCreature.OnHear -= creatureHearEventHandler.Execute;
            }
        }
    }
}