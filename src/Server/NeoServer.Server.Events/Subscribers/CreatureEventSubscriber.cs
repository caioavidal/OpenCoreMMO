using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Events.Combat;
using NeoServer.Server.Events.Creature;
using NeoServer.Server.Events.Creature.Npcs;

namespace NeoServer.Server.Events
{
    public class CreatureEventSubscriber : ICreatureEventSubscriber
    {
        private readonly CreatureAttackEventHandler _creatureAttackEventHandler;
        private readonly CreatureBlockedAttackEventHandler _creatureBlockedAttackEventHandler;
        private readonly CreatureChangedAttackTargetEventHandler _creatureChangedAttackTargetEventHandler;
        private readonly CreatureChangedSpeedEventHandler _creatureChangedSpeedEventHandler;
        private readonly CreatureHealedEventHandler _creatureHealedEventHandler;
        private readonly CreatureKilledEventHandler _creatureKilledEventHandler;
        private readonly CreatureInjuredEventHandler _creatureReceiveDamageEventHandler;
        private readonly CreatureStartedFollowingEventHandler _creatureStartedFollowingEventHandler;
        private readonly CreatureStartedWalkingEventHandler _creatureStartedWalkingEventHandler;
        private readonly CreatureTurnedToDirectionEventHandler _creatureTurnToDirectionEventHandler;
        private readonly CreatureWasBornEventHandler _creatureWasBornEventHandler;
        private readonly CreatureChangedOutfitEventHandler creatureChangedOutfitEventHandler;
        private readonly CreatureHearEventHandler creatureHearEventHandler;
        private readonly PlayerSentMessageEventHandler creatureSentMessageEventHandler;
        private readonly CreatureChangedVisibilityEventHandler creatureTurnedInvisibleEventHandler;
        private readonly NpcShowShopEventHandler npcShowShopEventHandler;

        public CreatureEventSubscriber(CreatureInjuredEventHandler creatureReceiveDamageEventHandler,
            CreatureKilledEventHandler creatureKilledEventHandler,
            CreatureWasBornEventHandler creatureWasBornEventHandler,
            CreatureBlockedAttackEventHandler creatureBlockedAttackEventHandler,
            CreatureAttackEventHandler creatureAttackEventHandler,
            CreatureTurnedToDirectionEventHandler creatureTurnToDirectionEventHandler,
            CreatureStartedWalkingEventHandler creatureStartedWalkingEventHandler,
            CreatureHealedEventHandler creatureHealedEventHandler,
            CreatureChangedAttackTargetEventHandler creatureChangedAttackTargetEventHandler,
            CreatureStartedFollowingEventHandler creatureStartedFollowingEventHandler,
            CreatureChangedSpeedEventHandler creatureChangedSpeedEventHandler,
            CreatureHearEventHandler creatureHearEventHandler,
            CreatureChangedVisibilityEventHandler creatureTurnedInvisibleEventHandler,
            CreatureChangedOutfitEventHandler creatureChangedOutfitEventHandler,
            PlayerSentMessageEventHandler creatureSentMessageEventHandler,
            NpcShowShopEventHandler npcShowShopEventHandler)
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
            this.npcShowShopEventHandler = npcShowShopEventHandler;
        }

        public void Subscribe(ICreature creature)
        {
            creature.OnChangedOutfit += creatureChangedOutfitEventHandler.Execute;

            if (creature is ISociableCreature sociableCreature)
                sociableCreature.OnHear += creatureHearEventHandler.Execute;
            if (creature is ICombatActor combatActor)
            {
                combatActor.OnTargetChanged += _creatureChangedAttackTargetEventHandler.Execute;
                combatActor.OnDamaged += _creatureReceiveDamageEventHandler.Execute;
                combatActor.OnKilled += _creatureKilledEventHandler.Execute;
                combatActor.OnBlockedAttack += _creatureBlockedAttackEventHandler.Execute;
                combatActor.OnAttackEnemy += _creatureAttackEventHandler.Execute;
                combatActor.OnHeal += _creatureHealedEventHandler.Execute;
                combatActor.OnChangedVisibility += creatureTurnedInvisibleEventHandler.Execute;
            }

            if (creature is IShopperNpc shopperNpc) shopperNpc.OnShowShop += npcShowShopEventHandler.Execute;

            #region WalkableEvents

            if (creature is IWalkableCreature walkableCreature)
            {
                walkableCreature.OnStartedFollowing += _creatureStartedFollowingEventHandler.Execute;
                walkableCreature.OnChangedSpeed += _creatureChangedSpeedEventHandler.Execute;
                walkableCreature.OnStartedWalking += _creatureStartedWalkingEventHandler.Execute;
                walkableCreature.OnTurnedToDirection += _creatureTurnToDirectionEventHandler.Execute;
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
                combatActor.OnAttackEnemy -= _creatureAttackEventHandler.Execute;
                combatActor.OnHeal -= _creatureHealedEventHandler.Execute;
                combatActor.OnChangedVisibility -= creatureTurnedInvisibleEventHandler.Execute;
            }

            if (creature is IWalkableCreature walkableCreature)
            {
                walkableCreature.OnStartedFollowing -= _creatureStartedFollowingEventHandler.Execute;
                walkableCreature.OnChangedSpeed -= _creatureChangedSpeedEventHandler.Execute;
                walkableCreature.OnTurnedToDirection -= _creatureTurnToDirectionEventHandler.Execute;
                walkableCreature.OnStartedWalking -= _creatureStartedWalkingEventHandler.Execute;
            }

            if (creature is ISociableCreature sociableCreature)
                sociableCreature.OnHear -= creatureHearEventHandler.Execute;
            if (creature is IShopperNpc shopperNpc) shopperNpc.OnShowShop -= npcShowShopEventHandler.Execute;
        }
    }
}