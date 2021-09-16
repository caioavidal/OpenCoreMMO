using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Server.Events.Chat;
using NeoServer.Server.Events.Combat;
using NeoServer.Server.Events.Items;
using NeoServer.Server.Events.Player;
using NeoServer.Server.Events.Player.Party;

namespace NeoServer.Server.Events.Subscribers
{
    public class PlayerEventSubscriber : ICreatureEventSubscriber
    {
        public PlayerEventSubscriber(PlayerWalkCancelledEventHandler playerWalkCancelledEventHandler,
            PlayerClosedContainerEventHandler playerClosedContainerEventHandler,
            PlayerOpenedContainerEventHandler playerOpenedContainerEventHandler,
            ContentModifiedOnContainerEventHandler contentModifiedOnContainerEventHandler,
            PlayerChangedInventoryEventHandler itemAddedToInventoryEventHandler,
            InvalidOperationEventHandler invalidOperationEventHandler,
            CreatureStoppedAttackEventHandler creatureStopedAttackEventHandler,
            PlayerGainedExperienceEventHandler playerGainedExperienceEventHandler,
            PlayerManaChangedEventHandler playerManaReducedEventHandler,
            SpellInvokedEventHandler playerUsedSpellEventHandler,
            PlayerCannotUseSpellEventHandler playerCannotUseSpellEventHandler,
            PlayerConditionChangedEventHandler playerConditionChangedEventHandler,
            PlayerLevelAdvancedEventHandler playerLevelAdvancedEventHandler,
            PlayerOperationFailedEventHandler playerOperationFailedEventHandler,
            PlayerLookedAtEventHandler playerLookedAtEventHandler,
            PlayerUpdatedSkillPointsEventHandler playerUpdatedSkillPointsEventHandler,
            PlayerUsedItemEventHandler playerUsedItemEventHandler,
            PlayerSelfAppearOnMapEventHandler playerSelfAppearOnMapEventHandler,
            PlayerJoinedChannelEventHandler playerJoinedChannelEventHandler,
            PlayerExitedChannelEventHandler playerExitedChannelEventHandler,
            PlayerAddToVipListEventHandler playerAddedToVipListEventHandler,
            PlayerLoadedVipListEventHandler playerLoadedVipListEvent,
            PlayerChangedOnlineStatusEventHandler playerChangedOnlineStatusEventHandler,
            PlayerSentMessageEventHandler playerSentMessageEventHandler,
            PlayerInviteToPartyEventHandler playerInviteToPartyEventHandler,
            PlayerRevokedPartyInviteEventHandler playerRevokedPartyInviteEventHandler,
            PlayerLeftPartyEventHandler playerLeftPartyEventHandler,
            PlayerInvitedToPartyEventHandler playerInvitedToPartyEventHandler,
            PlayerJoinedPartyEventHandler playerJoinedPartyEventHandler,
            PlayerPassedPartyLeadershipEventHandler playerPassedPartyLeadershipEventHandler,
            PlayerExhaustedEventHandler playerExhaustedEventHandler)
        {
            _playerWalkCancelledEventHandler = playerWalkCancelledEventHandler;
            _playerClosedContainerEventHandler = playerClosedContainerEventHandler;
            _playerOpenedContainerEventHandler = playerOpenedContainerEventHandler;
            _contentModifiedOnContainerEventHandler = contentModifiedOnContainerEventHandler;
            _itemAddedToInventoryEventHandler = itemAddedToInventoryEventHandler;
            _invalidOperationEventHandler = invalidOperationEventHandler;
            _creatureStopedAttackEventHandler = creatureStopedAttackEventHandler;
            _playerGainedExperienceEventHandler = playerGainedExperienceEventHandler;
            _playerManaReducedEventHandler = playerManaReducedEventHandler;
            _playerUsedSpellEventHandler = playerUsedSpellEventHandler;
            _playerCannotUseSpellEventHandler = playerCannotUseSpellEventHandler;
            _playerConditionChangedEventHandler = playerConditionChangedEventHandler;
            _playerLevelAdvancedEventHandler = playerLevelAdvancedEventHandler;
            _playerOperationFailedEventHandler = playerOperationFailedEventHandler;
            _playerLookedAtEventHandler = playerLookedAtEventHandler;
            _playerUpdatedSkillPointsEventHandler = playerUpdatedSkillPointsEventHandler;
            _playerUsedItemEventHandler = playerUsedItemEventHandler;
            _playerSelfAppearOnMapEventHandler = playerSelfAppearOnMapEventHandler;
            _playerJoinedChannelEventHandler = playerJoinedChannelEventHandler;
            _playerExitedChannelEventHandler = playerExitedChannelEventHandler;
            _playerAddedToVipListEventHandler = playerAddedToVipListEventHandler;
            _playerLoadedVipListEvent = playerLoadedVipListEvent;
            _playerChangedOnlineStatusEventHandler = playerChangedOnlineStatusEventHandler;
            _playerSentMessageEventHandler = playerSentMessageEventHandler;
            _playerInviteToPartyEventHandler = playerInviteToPartyEventHandler;
            _playerRevokedPartyInviteEventHandler = playerRevokedPartyInviteEventHandler;
            _playerLeftPartyEventHandler = playerLeftPartyEventHandler;
            _playerInvitedToPartyEventHandler = playerInvitedToPartyEventHandler;
            _playerJoinedPartyEventHandler = playerJoinedPartyEventHandler;
            _playerPassedPartyLeadershipEventHandler = playerPassedPartyLeadershipEventHandler;
            _playerExhaustedEventHandler = playerExhaustedEventHandler;
        }

        public void Subscribe(ICreature creature)
        {
            if (creature is not IPlayer player) return;

            player.OnCancelledWalk += _playerWalkCancelledEventHandler.Execute;
            player.Containers.OnClosedContainer += _playerClosedContainerEventHandler.Execute;
            player.Containers.OnOpenedContainer += _playerOpenedContainerEventHandler.Execute;

            player.Containers.RemoveItemAction += (player, containerId, slotIndex, item) =>
                _contentModifiedOnContainerEventHandler.Execute(player, ContainerOperation.ItemRemoved, containerId,
                    slotIndex, item);

            player.Containers.AddItemAction += (player, containerId, item) =>
                _contentModifiedOnContainerEventHandler.Execute(player, ContainerOperation.ItemAdded, containerId, 0,
                    item);

            player.Containers.UpdateItemAction += (player, containerId, slotIndex, item, amount) =>
                _contentModifiedOnContainerEventHandler.Execute(player, ContainerOperation.ItemUpdated, containerId,
                    slotIndex, item);

            player.Inventory.OnItemAddedToSlot += (inventory, item, slot, amount) =>
                _itemAddedToInventoryEventHandler?.Execute(inventory.Owner, slot);
            player.Inventory.OnItemRemovedFromSlot += (inventory, item, slot, amount) =>
                _itemAddedToInventoryEventHandler?.Execute(inventory.Owner, slot);

            player.Inventory.OnFailedToAddToSlot += error => _invalidOperationEventHandler?.Execute(player, error);
            player.OnStoppedAttack += _creatureStopedAttackEventHandler.Execute;
            player.OnGainedExperience += _playerGainedExperienceEventHandler.Execute;

            player.OnStatusChanged += _playerManaReducedEventHandler.Execute;
            player.OnUsedSpell += _playerUsedSpellEventHandler.Execute;
            player.OnCannotUseSpell += _playerCannotUseSpellEventHandler.Execute;
            player.OnAddedCondition += _playerConditionChangedEventHandler.Execute;
            player.OnRemovedCondition += _playerConditionChangedEventHandler.Execute;
            player.OnLevelAdvanced += _playerLevelAdvancedEventHandler.Execute;
            player.OnLookedAt += _playerLookedAtEventHandler.Execute;
            player.OnGainedSkillPoint += _playerUpdatedSkillPointsEventHandler.Execute;
            player.OnUsedItem += _playerUsedItemEventHandler.Execute;
            player.OnLoggedIn += _playerSelfAppearOnMapEventHandler.Execute;
            player.OnJoinedChannel += _playerJoinedChannelEventHandler.Execute;
            player.OnExitedChannel += _playerExitedChannelEventHandler.Execute;
            player.OnAddedToVipList += _playerAddedToVipListEventHandler.Execute;
            player.OnLoadedVipList += _playerLoadedVipListEvent.Execute;
            player.OnChangedOnlineStatus += _playerChangedOnlineStatusEventHandler.Execute;
            player.OnSentMessage += _playerSentMessageEventHandler.Execute;
            player.OnInviteToParty += _playerInviteToPartyEventHandler.Execute;
            player.OnRevokePartyInvite += _playerRevokedPartyInviteEventHandler.Execute;
            player.OnLeftParty += _playerLeftPartyEventHandler.Execute;
            player.OnInvitedToParty += _playerInvitedToPartyEventHandler.Execute;
            player.OnRejectedPartyInvite += _playerLeftPartyEventHandler.Execute;
            player.OnJoinedParty += _playerJoinedPartyEventHandler.Execute;
            player.OnPassedPartyLeadership += _playerPassedPartyLeadershipEventHandler.Execute;
            player.OnExhausted += _playerExhaustedEventHandler.Execute;
            player.OnAddedSkillBonus += _playerUpdatedSkillPointsEventHandler.Execute;
            player.OnRemovedSkillBonus += _playerUpdatedSkillPointsEventHandler.Execute;

        }

        public void Unsubscribe(ICreature creature)
        {
            if (creature is not IPlayer player) return;

            player.OnCancelledWalk -= _playerWalkCancelledEventHandler.Execute;
            player.Containers.OnClosedContainer -= _playerClosedContainerEventHandler.Execute;
            player.Containers.OnOpenedContainer -= _playerOpenedContainerEventHandler.Execute;

            player.Containers.RemoveItemAction -= (player, containerId, slotIndex, item) =>
                _contentModifiedOnContainerEventHandler.Execute(player, ContainerOperation.ItemRemoved, containerId,
                    slotIndex, item);

            player.Containers.AddItemAction -= (player, containerId, item) =>
                _contentModifiedOnContainerEventHandler.Execute(player, ContainerOperation.ItemAdded, containerId, 0,
                    item);

            player.Containers.UpdateItemAction -= (player, containerId, slotIndex, item, amount) =>
                _contentModifiedOnContainerEventHandler.Execute(player, ContainerOperation.ItemUpdated, containerId,
                    slotIndex, item);

            player.Inventory.OnItemAddedToSlot -= (inventory, item, slot, amount) =>
                _itemAddedToInventoryEventHandler.Execute(inventory.Owner, slot);
            player.Inventory.OnItemRemovedFromSlot -= (inventory, item, slot, amount) =>
                _itemAddedToInventoryEventHandler?.Execute(inventory.Owner, slot);

            player.Inventory.OnFailedToAddToSlot -= error => _invalidOperationEventHandler?.Execute(player, error);
            player.OnStoppedAttack -= _creatureStopedAttackEventHandler.Execute;
            player.OnGainedExperience -= _playerGainedExperienceEventHandler.Execute;

            player.OnStatusChanged -= _playerManaReducedEventHandler.Execute;
            player.OnUsedSpell -= _playerUsedSpellEventHandler.Execute;
            player.OnCannotUseSpell -= _playerCannotUseSpellEventHandler.Execute;
            player.OnAddedCondition -= _playerConditionChangedEventHandler.Execute;
            player.OnRemovedCondition -= _playerConditionChangedEventHandler.Execute;
            player.OnLevelAdvanced -= _playerLevelAdvancedEventHandler.Execute;
            player.OnLookedAt -= _playerLookedAtEventHandler.Execute;
            player.OnGainedSkillPoint -= _playerUpdatedSkillPointsEventHandler.Execute;
            player.OnUsedItem -= _playerUsedItemEventHandler.Execute;
            player.OnLoggedIn -= _playerSelfAppearOnMapEventHandler.Execute;
            player.OnJoinedChannel -= _playerJoinedChannelEventHandler.Execute;
            player.OnExitedChannel -= _playerExitedChannelEventHandler.Execute;
            player.OnAddedToVipList -= _playerAddedToVipListEventHandler.Execute;
            player.OnLoadedVipList -= _playerLoadedVipListEvent.Execute;
            player.OnChangedOnlineStatus -= _playerChangedOnlineStatusEventHandler.Execute;
            player.OnSentMessage -= _playerSentMessageEventHandler.Execute;
            player.OnInviteToParty -= _playerInviteToPartyEventHandler.Execute;
            player.OnRevokePartyInvite -= _playerRevokedPartyInviteEventHandler.Execute;
            player.OnLeftParty -= _playerLeftPartyEventHandler.Execute;
            player.OnInvitedToParty -= _playerInvitedToPartyEventHandler.Execute;
            player.OnJoinedParty -= _playerJoinedPartyEventHandler.Execute;
            player.OnPassedPartyLeadership -= _playerPassedPartyLeadershipEventHandler.Execute;

            player.OnAddedSkillBonus -= _playerUpdatedSkillPointsEventHandler.Execute;
            player.OnRemovedSkillBonus += _playerUpdatedSkillPointsEventHandler.Execute;
        }

        #region event handlers

        private readonly PlayerWalkCancelledEventHandler _playerWalkCancelledEventHandler;
        private readonly PlayerClosedContainerEventHandler _playerClosedContainerEventHandler;
        private readonly PlayerOpenedContainerEventHandler _playerOpenedContainerEventHandler;
        private readonly ContentModifiedOnContainerEventHandler _contentModifiedOnContainerEventHandler;
        private readonly PlayerChangedInventoryEventHandler _itemAddedToInventoryEventHandler;
        private readonly InvalidOperationEventHandler _invalidOperationEventHandler;
        private readonly CreatureStoppedAttackEventHandler _creatureStopedAttackEventHandler;
        private readonly PlayerGainedExperienceEventHandler _playerGainedExperienceEventHandler;
        private readonly PlayerManaChangedEventHandler _playerManaReducedEventHandler;
        private readonly SpellInvokedEventHandler _playerUsedSpellEventHandler;
        private readonly PlayerCannotUseSpellEventHandler _playerCannotUseSpellEventHandler;
        private readonly PlayerConditionChangedEventHandler _playerConditionChangedEventHandler;
        private readonly PlayerLevelAdvancedEventHandler _playerLevelAdvancedEventHandler;
        private readonly PlayerOperationFailedEventHandler _playerOperationFailedEventHandler;
        private readonly PlayerLookedAtEventHandler _playerLookedAtEventHandler;
        private readonly PlayerUpdatedSkillPointsEventHandler _playerUpdatedSkillPointsEventHandler;
        private readonly PlayerUsedItemEventHandler _playerUsedItemEventHandler;
        private readonly PlayerSelfAppearOnMapEventHandler _playerSelfAppearOnMapEventHandler;
        private readonly PlayerJoinedChannelEventHandler _playerJoinedChannelEventHandler;
        private readonly PlayerExitedChannelEventHandler _playerExitedChannelEventHandler;
        private readonly PlayerAddToVipListEventHandler _playerAddedToVipListEventHandler;
        private readonly PlayerLoadedVipListEventHandler _playerLoadedVipListEvent;
        private readonly PlayerChangedOnlineStatusEventHandler _playerChangedOnlineStatusEventHandler;
        private readonly PlayerSentMessageEventHandler _playerSentMessageEventHandler;
        private readonly PlayerInviteToPartyEventHandler _playerInviteToPartyEventHandler;
        private readonly PlayerRevokedPartyInviteEventHandler _playerRevokedPartyInviteEventHandler;
        private readonly PlayerLeftPartyEventHandler _playerLeftPartyEventHandler;
        private readonly PlayerInvitedToPartyEventHandler _playerInvitedToPartyEventHandler;
        private readonly PlayerJoinedPartyEventHandler _playerJoinedPartyEventHandler;
        private readonly PlayerPassedPartyLeadershipEventHandler _playerPassedPartyLeadershipEventHandler;
        private readonly PlayerExhaustedEventHandler _playerExhaustedEventHandler;

        #endregion
    }
}