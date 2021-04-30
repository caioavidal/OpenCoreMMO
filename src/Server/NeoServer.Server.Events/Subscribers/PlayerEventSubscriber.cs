using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Events.Creature;
using NeoServer.Server.Events.Player;
using NeoServer.Server.Events.Player.Party;

namespace NeoServer.Server.Events
{
    public class PlayerEventSubscriber : ICreatureEventSubscriber
    {
        #region event handlers
        private readonly PlayerWalkCancelledEventHandler playerWalkCancelledEventHandler;
        private readonly PlayerClosedContainerEventHandler playerClosedContainerEventHandler;
        private readonly PlayerOpenedContainerEventHandler playerOpenedContainerEventHandler;
        private readonly ContentModifiedOnContainerEventHandler contentModifiedOnContainerEventHandler;
        private readonly PlayerChangedInventoryEventHandler itemAddedToInventoryEventHandler;
        private readonly InvalidOperationEventHandler invalidOperationEventHandler;
        private readonly CreatureStoppedAttackEventHandler creatureStopedAttackEventHandler;
        private readonly PlayerGainedExperienceEventHandler _playerGainedExperienceEventHandler;
        private readonly PlayerManaChangedEventHandler _playerManaReducedEventHandler;
        private readonly SpellInvokedEventHandler _playerUsedSpellEventHandler;
        private readonly PlayerCannotUseSpellEventHandler _playerCannotUseSpellEventHandler;
        private readonly PlayerConditionChangedEventHandler _playerConditionChangedEventHandler;
        private readonly PlayerLevelAdvancedEventHandler _playerLevelAdvancedEventHandler;
        private readonly PlayerOperationFailedEventHandler _playerOperationFailedEventHandler;
        private readonly PlayerLookedAtEventHandler playerLookedAtEventHandler;
        private readonly PlayerGainedSkillPointsEventHandler playerGainedSkillPointsEventHandler;
        private readonly PlayerUsedItemEventHandler playerUsedItemEventHandler;
        private readonly PlayerSelfAppearOnMapEventHandler playerSelfAppearOnMapEventHandler;
        private readonly PlayerJoinedChannelEventHandler playerJoinedChannelEventHandler;
        private readonly PlayerExitedChannelEventHandler playerExitedChannelEventHandler;
        private readonly PlayerAddToVipListEventHandler playerAddedToVipListEventHandler;
        private readonly PlayerLoadedVipListEventHandler playerLoadedVipListEvent;
        private readonly PlayerChangedOnlineStatusEventHandler playerChangedOnlineStatusEventHandler;
        private readonly PlayerSentMessageEventHandler playerSentMessageEventHandler;
        private readonly PlayerInviteToPartyEventHandler playerInviteToPartyEventHandler;
        private readonly PlayerRevokedPartyInviteEventHandler playerRevokedPartyInviteEventHandler;
        private readonly PlayerLeftPartyEventHandler playerLeftPartyEventHandler;
        private readonly PlayerInvitedToPartyEventHandler playerInvitedToPartyEventHandler;
        private readonly PlayerJoinedPartyEventHandler playerJoinedPartyEventHandler;
        private readonly PlayerPassedPartyLeadershipEventHandler playerPassedPartyLeadershipEventHandler;
        #endregion

        public PlayerEventSubscriber(PlayerWalkCancelledEventHandler playerWalkCancelledEventHandler, PlayerClosedContainerEventHandler playerClosedContainerEventHandler,
            PlayerOpenedContainerEventHandler playerOpenedContainerEventHandler, ContentModifiedOnContainerEventHandler contentModifiedOnContainerEventHandler,
            PlayerChangedInventoryEventHandler itemAddedToInventoryEventHandler, InvalidOperationEventHandler invalidOperationEventHandler,
            CreatureStoppedAttackEventHandler creatureStopedAttackEventHandler, PlayerGainedExperienceEventHandler playerGainedExperienceEventHandler,
            PlayerManaChangedEventHandler playerManaReducedEventHandler, SpellInvokedEventHandler playerUsedSpellEventHandler, PlayerCannotUseSpellEventHandler playerCannotUseSpellEventHandler,
            PlayerConditionChangedEventHandler playerConditionChangedEventHandler, PlayerLevelAdvancedEventHandler playerLevelAdvancedEventHandler,
            PlayerOperationFailedEventHandler playerOperationFailedEventHandler, PlayerLookedAtEventHandler playerLookedAtEventHandler,
            PlayerGainedSkillPointsEventHandler playerGainedSkillPointsEventHandler, PlayerUsedItemEventHandler playerUsedItemEventHandler,
            PlayerSelfAppearOnMapEventHandler playerSelfAppearOnMapEventHandler, PlayerJoinedChannelEventHandler playerJoinedChannelEventHandler,
            PlayerExitedChannelEventHandler playerExitedChannelEventHandler, PlayerAddToVipListEventHandler playerAddedToVipListEventHandler,
            PlayerLoadedVipListEventHandler playerLoadedVipListEvent, PlayerChangedOnlineStatusEventHandler playerChangedOnlineStatusEventHandler,
            PlayerSentMessageEventHandler playerSentMessageEventHandler, PlayerInviteToPartyEventHandler playerInviteToPartyEventHandler,
            PlayerRevokedPartyInviteEventHandler playerRevokedPartyInviteEventHandler, PlayerLeftPartyEventHandler playerLeftPartyEventHandler,
            PlayerInvitedToPartyEventHandler playerInvitedToPartyEventHandler, PlayerJoinedPartyEventHandler playerJoinedPartyEventHandler,
            PlayerPassedPartyLeadershipEventHandler playerPassedPartyLeadershipEventHandler)
        {
            this.playerWalkCancelledEventHandler = playerWalkCancelledEventHandler;
            this.playerClosedContainerEventHandler = playerClosedContainerEventHandler;
            this.playerOpenedContainerEventHandler = playerOpenedContainerEventHandler;
            this.contentModifiedOnContainerEventHandler = contentModifiedOnContainerEventHandler;
            this.itemAddedToInventoryEventHandler = itemAddedToInventoryEventHandler;
            this.invalidOperationEventHandler = invalidOperationEventHandler;
            this.creatureStopedAttackEventHandler = creatureStopedAttackEventHandler;
            _playerGainedExperienceEventHandler = playerGainedExperienceEventHandler;
            _playerManaReducedEventHandler = playerManaReducedEventHandler;
            _playerUsedSpellEventHandler = playerUsedSpellEventHandler;
            _playerCannotUseSpellEventHandler = playerCannotUseSpellEventHandler;
            _playerConditionChangedEventHandler = playerConditionChangedEventHandler;
            _playerLevelAdvancedEventHandler = playerLevelAdvancedEventHandler;
            _playerOperationFailedEventHandler = playerOperationFailedEventHandler;
            this.playerLookedAtEventHandler = playerLookedAtEventHandler;
            this.playerGainedSkillPointsEventHandler = playerGainedSkillPointsEventHandler;
            this.playerUsedItemEventHandler = playerUsedItemEventHandler;
            this.playerSelfAppearOnMapEventHandler = playerSelfAppearOnMapEventHandler;
            this.playerJoinedChannelEventHandler = playerJoinedChannelEventHandler;
            this.playerExitedChannelEventHandler = playerExitedChannelEventHandler;
            this.playerAddedToVipListEventHandler = playerAddedToVipListEventHandler;
            this.playerLoadedVipListEvent = playerLoadedVipListEvent;
            this.playerChangedOnlineStatusEventHandler = playerChangedOnlineStatusEventHandler;
            this.playerSentMessageEventHandler = playerSentMessageEventHandler;
            this.playerInviteToPartyEventHandler = playerInviteToPartyEventHandler;
            this.playerRevokedPartyInviteEventHandler = playerRevokedPartyInviteEventHandler;
            this.playerLeftPartyEventHandler = playerLeftPartyEventHandler;
            this.playerInvitedToPartyEventHandler = playerInvitedToPartyEventHandler;
            this.playerJoinedPartyEventHandler = playerJoinedPartyEventHandler;
            this.playerPassedPartyLeadershipEventHandler = playerPassedPartyLeadershipEventHandler;
        }

        public void Subscribe(ICreature creature)
        {
            if (creature is not IPlayer player) return;

            player.OnCancelledWalk += playerWalkCancelledEventHandler.Execute;
            player.Containers.OnClosedContainer += playerClosedContainerEventHandler.Execute;
            player.Containers.OnOpenedContainer += playerOpenedContainerEventHandler.Execute;

            player.Containers.RemoveItemAction += (player, containerId, slotIndex, item) =>
             contentModifiedOnContainerEventHandler.Execute(player, ContainerOperation.ItemRemoved, containerId, slotIndex, item);

            player.Containers.AddItemAction += (player, containerId, item) =>
            contentModifiedOnContainerEventHandler.Execute(player, ContainerOperation.ItemAdded, containerId, 0, item);

            player.Containers.UpdateItemAction += (player, containerId, slotIndex, item, amount) =>
                contentModifiedOnContainerEventHandler.Execute(player, ContainerOperation.ItemUpdated, containerId, slotIndex, item);

            player.Inventory.OnItemAddedToSlot += (inventory, item, slot, amount) => itemAddedToInventoryEventHandler?.Execute(inventory.Owner, slot);
            player.Inventory.OnItemRemovedFromSlot += (inventory, item, slot, amount) => itemAddedToInventoryEventHandler?.Execute(inventory.Owner, slot);

            player.Inventory.OnFailedToAddToSlot += (error) => invalidOperationEventHandler?.Execute(player, error);
            player.OnStoppedAttack += creatureStopedAttackEventHandler.Execute;
            player.OnGainedExperience += _playerGainedExperienceEventHandler.Execute;

            player.OnStatusChanged += _playerManaReducedEventHandler.Execute;
            player.OnUsedSpell += _playerUsedSpellEventHandler.Execute;
            player.OnCannotUseSpell += _playerCannotUseSpellEventHandler.Execute;
            player.OnAddedCondition += _playerConditionChangedEventHandler.Execute;
            player.OnRemovedCondition += _playerConditionChangedEventHandler.Execute;
            player.OnLevelAdvanced += _playerLevelAdvancedEventHandler.Execute;
            player.OnLookedAt += playerLookedAtEventHandler.Execute;
            player.OnGainedSkillPoint += playerGainedSkillPointsEventHandler.Execute;
            player.OnUsedItem += playerUsedItemEventHandler.Execute;
            player.OnLoggedIn += playerSelfAppearOnMapEventHandler.Execute;
            player.OnJoinedChannel += playerJoinedChannelEventHandler.Execute;
            player.OnExitedChannel += playerExitedChannelEventHandler.Execute;
            player.OnAddedToVipList += playerAddedToVipListEventHandler.Execute;
            player.OnLoadedVipList += playerLoadedVipListEvent.Execute;
            player.OnChangedOnlineStatus += playerChangedOnlineStatusEventHandler.Execute;
            player.OnSentMessage += playerSentMessageEventHandler.Execute;
            player.OnInviteToParty += playerInviteToPartyEventHandler.Execute;
            player.OnRevokePartyInvite += playerRevokedPartyInviteEventHandler.Execute;
            player.OnLeftParty += playerLeftPartyEventHandler.Execute;
            player.OnInvitedToParty += playerInvitedToPartyEventHandler.Execute;
            player.OnRejectedPartyInvite += playerLeftPartyEventHandler.Execute;
            player.OnJoinedParty += playerJoinedPartyEventHandler.Execute;
            player.OnPassedPartyLeadership += playerPassedPartyLeadershipEventHandler.Execute;
        }

        public void Unsubscribe(ICreature creature)
        {
            if (creature is not IPlayer player) return;

            player.OnCancelledWalk -= playerWalkCancelledEventHandler.Execute;
            player.Containers.OnClosedContainer -= playerClosedContainerEventHandler.Execute;
            player.Containers.OnOpenedContainer -= playerOpenedContainerEventHandler.Execute;

            player.Containers.RemoveItemAction -= (player, containerId, slotIndex, item) =>
             contentModifiedOnContainerEventHandler.Execute(player, ContainerOperation.ItemRemoved, containerId, slotIndex, item);

            player.Containers.AddItemAction -= (player, containerId, item) =>
            contentModifiedOnContainerEventHandler.Execute(player, ContainerOperation.ItemAdded, containerId, 0, item);

            player.Containers.UpdateItemAction -= (player, containerId, slotIndex, item, amount) =>
                contentModifiedOnContainerEventHandler.Execute(player, ContainerOperation.ItemUpdated, containerId, slotIndex, item);

            player.Inventory.OnItemAddedToSlot -= (inventory, item, slot, amount) => itemAddedToInventoryEventHandler?.Execute(inventory.Owner, slot);
            player.Inventory.OnItemRemovedFromSlot -= (inventory, item, slot, amount) => itemAddedToInventoryEventHandler?.Execute(inventory.Owner, slot);

            player.Inventory.OnFailedToAddToSlot -= (error) => invalidOperationEventHandler?.Execute(player, error);
            player.OnStoppedAttack -= creatureStopedAttackEventHandler.Execute;
            player.OnGainedExperience -= _playerGainedExperienceEventHandler.Execute;

            player.OnStatusChanged -= _playerManaReducedEventHandler.Execute;
            player.OnUsedSpell -= _playerUsedSpellEventHandler.Execute;
            player.OnCannotUseSpell -= _playerCannotUseSpellEventHandler.Execute;
            player.OnAddedCondition -= _playerConditionChangedEventHandler.Execute;
            player.OnRemovedCondition -= _playerConditionChangedEventHandler.Execute;
            player.OnLevelAdvanced -= _playerLevelAdvancedEventHandler.Execute;
            player.OnLookedAt -= playerLookedAtEventHandler.Execute;
            player.OnGainedSkillPoint -= playerGainedSkillPointsEventHandler.Execute;
            player.OnUsedItem -= playerUsedItemEventHandler.Execute;
            player.OnLoggedIn -= playerSelfAppearOnMapEventHandler.Execute;
            player.OnJoinedChannel -= playerJoinedChannelEventHandler.Execute;
            player.OnExitedChannel -= playerExitedChannelEventHandler.Execute;
            player.OnAddedToVipList -= playerAddedToVipListEventHandler.Execute;
            player.OnLoadedVipList -= playerLoadedVipListEvent.Execute;
            player.OnChangedOnlineStatus -= playerChangedOnlineStatusEventHandler.Execute;
            player.OnSentMessage -= playerSentMessageEventHandler.Execute;
            player.OnInviteToParty -= playerInviteToPartyEventHandler.Execute;
            player.OnRevokePartyInvite -= playerRevokedPartyInviteEventHandler.Execute;
            player.OnLeftParty -= playerLeftPartyEventHandler.Execute;
            player.OnInvitedToParty -= playerInvitedToPartyEventHandler.Execute;
            player.OnJoinedParty -= playerJoinedPartyEventHandler.Execute;
            player.OnPassedPartyLeadership -= playerPassedPartyLeadershipEventHandler.Execute;
        }
    }
}
