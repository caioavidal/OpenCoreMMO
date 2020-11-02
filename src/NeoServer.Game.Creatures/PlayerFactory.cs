using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Enums.Players;
using NeoServer.Server.Events;
using NeoServer.Server.Events.Player;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeoServer.Game.Creatures
{
    public class PlayerFactory : IPlayerFactory
    {
        private readonly Func<ushort, Location, IDictionary<ItemAttribute, IConvertible>, IItem> itemFactory;
        private readonly PlayerWalkCancelledEventHandler playerWalkCancelledEventHandler;
        private readonly PlayerClosedContainerEventHandler playerClosedContainerEventHandler;
        private readonly PlayerOpenedContainerEventHandler playerOpenedContainerEventHandler;
        private readonly ContentModifiedOnContainerEventHandler contentModifiedOnContainerEventHandler;
        private readonly ItemAddedToInventoryEventHandler itemAddedToInventoryEventHandler;
        private readonly InvalidOperationEventHandler invalidOperationEventHandler;
        private readonly CreatureStoppedAttackEventHandler creatureStopedAttackEventHandler;
        private readonly PlayerGainedExperienceEventHandler _playerGainedExperienceEventHandler;
        private readonly PlayerManaReducedEventHandler _playerManaReducedEventHandler;
        private readonly PlayerUsedSpellEventHandler _playerUsedSpellEventHandler;
        private readonly PlayerCannotUseSpellEventHandler _playerCannotUseSpellEventHandler;
        private readonly PlayerConditionChangedEventHandler _playerConditionChangedEventHandler;

        private readonly IPathFinder _pathFinder;

        public PlayerFactory(Func<ushort, Location, IDictionary<ItemAttribute, IConvertible>, IItem> itemFactory,
             PlayerWalkCancelledEventHandler playerWalkCancelledEventHandler,
            PlayerClosedContainerEventHandler playerClosedContainerEventHandler, PlayerOpenedContainerEventHandler playerOpenedContainerEventHandler,
            ContentModifiedOnContainerEventHandler contentModifiedOnContainerEventHandler, ItemAddedToInventoryEventHandler itemAddedToInventoryEventHandler,
            InvalidOperationEventHandler invalidOperationEventHandler, CreatureStoppedAttackEventHandler creatureStopedAttackEventHandler,
            PlayerGainedExperienceEventHandler playerGainedExperienceEventHandler, PlayerManaReducedEventHandler playerManaReducedEventHandler,
            IPathFinder pathFinder, PlayerUsedSpellEventHandler playerUsedSpellEventHandler, 
            PlayerCannotUseSpellEventHandler playerCannotUseSpellEventHandler, PlayerConditionChangedEventHandler playerConditionChangedEventHandler)
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
            _pathFinder = pathFinder;
            _playerUsedSpellEventHandler = playerUsedSpellEventHandler;
            _playerCannotUseSpellEventHandler = playerCannotUseSpellEventHandler;
            _playerConditionChangedEventHandler = playerConditionChangedEventHandler;
        }
        public IPlayer Create(IPlayerModel player)
        {
            var newPlayer = new Player(
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
                player.MaxSoulPoints,
                player.Skills,
                player.StaminaMinutes,
                player.Outfit,
                ConvertToInventory(player),
                player.Speed,
                player.Location,
                _pathFinder.Find
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

            newPlayer.Inventory.OnFailedToAddToSlot += (error) => invalidOperationEventHandler?.Execute(newPlayer, error);
            newPlayer.OnStoppedAttack += (actor) => creatureStopedAttackEventHandler?.Execute(newPlayer);
            newPlayer.OnGainedExperience += _playerGainedExperienceEventHandler.Execute;

            newPlayer.OnManaReduced += _playerManaReducedEventHandler.Execute;
            newPlayer.OnUsedSpell += _playerUsedSpellEventHandler.Execute;
            newPlayer.OnCannotUseSpell += _playerCannotUseSpellEventHandler.Execute;
            newPlayer.OnAddedCondition += _playerConditionChangedEventHandler.Execute;
            newPlayer.OnRemovedCondition += _playerConditionChangedEventHandler.Execute;

            return newPlayer;
        }
        private IDictionary<Slot, Tuple<IPickupable, ushort>> ConvertToInventory(IPlayerModel player)
        {
            var inventoryDic = new Dictionary<Slot, Tuple<IPickupable, ushort>>();
            foreach (var slot in player.Inventory)
            {
                if (!(itemFactory(slot.Value, player.Location, null) is IPickupable createdItem))
                {
                    continue;
                }

                if (slot.Key == Slot.Backpack)
                {
                    if (!(createdItem is IContainer container))
                    {
                        continue;
                    }

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

            var item = itemFactory(itemModel.ServerId, location, new Dictionary<ItemAttribute, IConvertible>()
                        {
                            {ItemAttribute.Count, itemModel.Amount }
                        });

            if (item is IContainer childrenContainer)
            {
                childrenContainer.SetParent(container);
                container.TryAddItem(BuildContainer(itemModel.Items?.Reverse().ToList(), 0, location, childrenContainer));
            }
            else
            {
                container.TryAddItem(item);

            }
            return BuildContainer(items, ++index, location, container);
        }

    }
}
