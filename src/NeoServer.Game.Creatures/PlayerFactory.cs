using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Players;
using NeoServer.Server.Events;
using NeoServer.Server.Events.Player;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Creatures.Vocations;

namespace NeoServer.Game.Creatures
{
    public class PlayerFactory : IPlayerFactory
    {
        private readonly IItemFactory itemFactory;
        private readonly PlayerWalkCancelledEventHandler playerWalkCancelledEventHandler;
        private readonly PlayerClosedContainerEventHandler playerClosedContainerEventHandler;
        private readonly PlayerOpenedContainerEventHandler playerOpenedContainerEventHandler;
        private readonly ContentModifiedOnContainerEventHandler contentModifiedOnContainerEventHandler;
        private readonly ItemAddedToInventoryEventHandler itemAddedToInventoryEventHandler;
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
        private readonly CreaturePathAccess creaturePathAccess;

        public PlayerFactory(IItemFactory itemFactory,
             PlayerWalkCancelledEventHandler playerWalkCancelledEventHandler,
            PlayerClosedContainerEventHandler playerClosedContainerEventHandler, PlayerOpenedContainerEventHandler playerOpenedContainerEventHandler,
            ContentModifiedOnContainerEventHandler contentModifiedOnContainerEventHandler, ItemAddedToInventoryEventHandler itemAddedToInventoryEventHandler,
            InvalidOperationEventHandler invalidOperationEventHandler, CreatureStoppedAttackEventHandler creatureStopedAttackEventHandler,
            PlayerGainedExperienceEventHandler playerGainedExperienceEventHandler, PlayerManaChangedEventHandler playerManaReducedEventHandler,
            SpellInvokedEventHandler playerUsedSpellEventHandler,
            PlayerCannotUseSpellEventHandler playerCannotUseSpellEventHandler, PlayerConditionChangedEventHandler playerConditionChangedEventHandler,
            PlayerLevelAdvancedEventHandler playerLevelAdvancedEventHandler, PlayerOperationFailedEventHandler playerOperationFailedEventHandler,
            PlayerLookedAtEventHandler playerLookedAtEventHandler, PlayerGainedSkillPointsEventHandler playerGainedSkillPointsEventHandler,
            CreaturePathAccess creaturePathAccess, PlayerUsedItemEventHandler playerUsedItemEventHandler)
        {
            this.itemFactory = itemFactory;
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
            this.creaturePathAccess = creaturePathAccess;
            this.playerUsedItemEventHandler = playerUsedItemEventHandler;
        }
        public IPlayer Create(IPlayerModel player)
        {
            if(!VocationStore.TryGetValue(player.Vocation, out var vocation))
            {
                throw new Exception("Player vocation not found");
            }
          
            var newPlayer = new Player(
                (uint)player.Id,
                player.CharacterName,
                player.ChaseMode,
                player.Capacity,
                player.HealthPoints,
                player.MaxHealthPoints,
                player.Vocation,
                player.Gender,
                player.Online,
                player.Mana,
                player.MaxMana,
                player.FightMode,
                player.SoulPoints,
                vocation.SoulMax,
                ConvertToSkills(player),
                player.StaminaMinutes,
                player.Outfit,
                ConvertToInventory(player),
                player.Speed,
                player.Location,
               creaturePathAccess
                );

            newPlayer.OnCancelledWalk += playerWalkCancelledEventHandler.Execute;
            newPlayer.Containers.OnClosedContainer += playerClosedContainerEventHandler.Execute;
            newPlayer.Containers.OnOpenedContainer += playerOpenedContainerEventHandler.Execute;

            newPlayer.Containers.RemoveItemAction += (player, containerId, slotIndex, item) =>
                contentModifiedOnContainerEventHandler.Execute(player, ContainerOperation.ItemRemoved, containerId, slotIndex, item);

            newPlayer.Containers.AddItemAction += (player, containerId, item) =>
                contentModifiedOnContainerEventHandler.Execute(player, ContainerOperation.ItemAdded, containerId, 0, item);

            newPlayer.Containers.UpdateItemAction += (player, containerId, slotIndex, item, amount) =>
                contentModifiedOnContainerEventHandler.Execute(player, ContainerOperation.ItemUpdated, containerId, slotIndex, item);

            newPlayer.Inventory.OnItemAddedToSlot += (inventory, item, slot, amount) => itemAddedToInventoryEventHandler?.Execute(inventory.Owner, slot);
            newPlayer.Inventory.OnItemRemovedFromSlot += (inventory, item, slot, amount) => itemAddedToInventoryEventHandler?.Execute(inventory.Owner, slot);

            newPlayer.Inventory.OnFailedToAddToSlot += (error) => invalidOperationEventHandler?.Execute(newPlayer, error);
            newPlayer.OnStoppedAttack += creatureStopedAttackEventHandler.Execute;
            newPlayer.OnGainedExperience += _playerGainedExperienceEventHandler.Execute;

            newPlayer.OnStatusChanged += _playerManaReducedEventHandler.Execute;
            newPlayer.OnUsedSpell += _playerUsedSpellEventHandler.Execute;
            newPlayer.OnCannotUseSpell += _playerCannotUseSpellEventHandler.Execute;
            newPlayer.OnAddedCondition += _playerConditionChangedEventHandler.Execute;
            newPlayer.OnRemovedCondition += _playerConditionChangedEventHandler.Execute;
            newPlayer.OnLevelAdvanced += _playerLevelAdvancedEventHandler.Execute;
            newPlayer.OnOperationFailed += _playerOperationFailedEventHandler.Execute;
            newPlayer.OnLookedAt += playerLookedAtEventHandler.Execute;
            newPlayer.OnGainedSkillPoint += playerGainedSkillPointsEventHandler.Execute;
            newPlayer.OnUsedItem += playerUsedItemEventHandler.Execute;
            return newPlayer;
        }

        private Dictionary<SkillType, ISkill> ConvertToSkills(IPlayerModel player)
        {
            VocationStore.TryGetValue(player.Vocation, out var vocation);
            return player.Skills.ToDictionary(x => x.Key, x=> (ISkill) new Skill(x.Key, vocation.Skill.ContainsKey((byte)x.Key) ? vocation.Skill[(byte)x.Key] : 1, (ushort)x.Value.Level, x.Value.Count));
        }
        private IDictionary<Slot, Tuple<IPickupable, ushort>> ConvertToInventory(IPlayerModel player)
        {
            var inventoryDic = new Dictionary<Slot, Tuple<IPickupable, ushort>>();
            foreach (var slot in player.Inventory)
            {
                if (!(itemFactory.Create(slot.Value, player.Location, null) is IPickupable createdItem))
                {
                    continue;
                }

                if (slot.Key == Slot.Backpack)
                {
                    if (createdItem is not IContainer container) continue;
                    BuildContainer(player.Items?.Reverse().ToList(), 0, player.Location, container);
                }

                inventoryDic.Add(slot.Key, new Tuple<IPickupable, ushort>(createdItem, slot.Value));

            }
            return inventoryDic;
        }

        public IContainer BuildContainer(IList<IItemModel> items, int index, Location location, IContainer container)
        {
            if (items == null || items.Count == index)
            {
                return container;
            }

            var itemModel = items[index];

            var item = itemFactory.Create(itemModel.ServerId, location, new Dictionary<ItemAttribute, IConvertible>()
                        {
                            {ItemAttribute.Count, itemModel.Amount }
                        });

            if (item is IContainer childrenContainer)
            {
                childrenContainer.SetParent(container);
                container.AddThing(BuildContainer(itemModel.Items?.Reverse().ToList(), 0, location, childrenContainer));
            }
            else
            {
                container.AddThing(item);

            }
            return BuildContainer(items, ++index, location, container);
        }

    }
}
