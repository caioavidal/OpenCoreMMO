using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Modules.Combat.Events;
using NeoServer.Modules.Creatures.Events;
using NeoServer.Modules.Shopping.OpenShop;

namespace NeoServer.Modules.Creatures;

public class CreatureEventSubscriber : ICreatureEventSubscriber, IServerEventSubscriber
{
    private readonly CreatureAttackEventHandler _creatureAttackEventHandler;
    private readonly CreatureBlockedAttackEventHandler _creatureBlockedAttackEventHandler;
    private readonly CreatureChangedAttackTargetEventHandler _creatureChangedAttackTargetEventHandler;
    private readonly CreatureChangedOutfitEventHandler _creatureChangedOutfitEventHandler;
    private readonly CreatureChangedSpeedEventHandler _creatureChangedSpeedEventHandler;
    private readonly CreatureHealedEventHandler _creatureHealedEventHandler;
    private readonly CreatureHearEventHandler _creatureHearEventHandler;
    private readonly CreatureKilledEventHandler _creatureKilledEventHandler;
    private readonly CreatureInjuredEventHandler _creatureReceiveDamageEventHandler;
    private readonly CreatureStartedFollowingEventHandler _creatureStartedFollowingEventHandler;
    private readonly CreatureStartedWalkingEventHandler _creatureStartedWalkingEventHandler;
    private readonly CreatureChangedVisibilityEventHandler _creatureTurnedInvisibleEventHandler;
    private readonly CreatureTurnedToDirectionEventHandler _creatureTurnToDirectionEventHandler;
    private readonly ShowShopEventHandler _showShopEventHandler;

    public CreatureEventSubscriber(CreatureInjuredEventHandler creatureReceiveDamageEventHandler,
        CreatureKilledEventHandler creatureKilledEventHandler,
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
        ShowShopEventHandler showShopEventHandler)
    {
        _creatureReceiveDamageEventHandler = creatureReceiveDamageEventHandler;
        _creatureKilledEventHandler = creatureKilledEventHandler;
        _creatureBlockedAttackEventHandler = creatureBlockedAttackEventHandler;
        _creatureAttackEventHandler = creatureAttackEventHandler;
        _creatureTurnToDirectionEventHandler = creatureTurnToDirectionEventHandler;
        _creatureStartedWalkingEventHandler = creatureStartedWalkingEventHandler;
        _creatureHealedEventHandler = creatureHealedEventHandler;
        _creatureChangedAttackTargetEventHandler = creatureChangedAttackTargetEventHandler;
        _creatureStartedFollowingEventHandler = creatureStartedFollowingEventHandler;
        _creatureChangedSpeedEventHandler = creatureChangedSpeedEventHandler;
        _creatureHearEventHandler = creatureHearEventHandler;
        _creatureTurnedInvisibleEventHandler = creatureTurnedInvisibleEventHandler;
        _creatureChangedOutfitEventHandler = creatureChangedOutfitEventHandler;
        _showShopEventHandler = showShopEventHandler;
    }

    public void Subscribe(ICreature creature)
    {
        creature.OnChangedOutfit += _creatureChangedOutfitEventHandler.Execute;

        if (creature is ISociableCreature sociableCreature)
            sociableCreature.OnHear += _creatureHearEventHandler.Execute;

        SubscribeToCombatActor(creature);

        if (creature is IShopperNpc shopperNpc) shopperNpc.OnShowShop += _showShopEventHandler.Execute;

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
        creature.OnChangedOutfit -= _creatureChangedOutfitEventHandler.Execute;

        if (creature is ICombatActor combatActor)
        {
            combatActor.OnTargetChanged -= _creatureChangedAttackTargetEventHandler.Execute;
            combatActor.OnInjured -= _creatureReceiveDamageEventHandler.Execute;
            combatActor.OnKilled -= _creatureKilledEventHandler.Execute;
            combatActor.OnBlockedAttack -= _creatureBlockedAttackEventHandler.Execute;
            combatActor.OnAttackEnemy -= _creatureAttackEventHandler.Execute;
            combatActor.OnHeal -= _creatureHealedEventHandler.Execute;
            combatActor.OnChangedVisibility -= _creatureTurnedInvisibleEventHandler.Execute;
        }

        if (creature is IWalkableCreature walkableCreature)
        {
            walkableCreature.OnStartedFollowing -= _creatureStartedFollowingEventHandler.Execute;
            walkableCreature.OnChangedSpeed -= _creatureChangedSpeedEventHandler.Execute;
            walkableCreature.OnTurnedToDirection -= _creatureTurnToDirectionEventHandler.Execute;
            walkableCreature.OnStartedWalking -= _creatureStartedWalkingEventHandler.Execute;
        }

        if (creature is ISociableCreature sociableCreature)
            sociableCreature.OnHear -= _creatureHearEventHandler.Execute;
        if (creature is IShopperNpc shopperNpc) shopperNpc.OnShowShop -= _showShopEventHandler.Execute;
    }

    private void SubscribeToCombatActor(ICreature creature)
    {
        if (creature is not ICombatActor combatActor) return;

        combatActor.OnTargetChanged += _creatureChangedAttackTargetEventHandler.Execute;
        combatActor.OnInjured += _creatureReceiveDamageEventHandler.Execute;
        combatActor.OnKilled += _creatureKilledEventHandler.Execute;
        combatActor.OnBlockedAttack += _creatureBlockedAttackEventHandler.Execute;
        combatActor.OnAttackEnemy += _creatureAttackEventHandler.Execute;
        combatActor.OnHeal += _creatureHealedEventHandler.Execute;
        combatActor.OnChangedVisibility += _creatureTurnedInvisibleEventHandler.Execute;
    }
}