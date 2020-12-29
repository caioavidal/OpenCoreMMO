using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Players;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Creatures.Vocations;

namespace NeoServer.Game.Creatures
{
    public class PlayerFactory : IPlayerFactory
    {
        private readonly IItemFactory itemFactory;

        private readonly CreaturePathAccess creaturePathAccess;

        public PlayerFactory(IItemFactory itemFactory, CreaturePathAccess creaturePathAccess)
        {
            this.itemFactory = itemFactory;
            this.creaturePathAccess = creaturePathAccess;
        }
        public IPlayer Create(IPlayerModel player)
        {
            if (!VocationStore.TryGetValue(player.Vocation, out var vocation))
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


            return newPlayer;
        }

        private Dictionary<SkillType, ISkill> ConvertToSkills(IPlayerModel player)
        {
            VocationStore.TryGetValue(player.Vocation, out var vocation);
            return player.Skills.ToDictionary(x => x.Key, x => (ISkill)new Skill(x.Key, vocation.Skill.ContainsKey((byte)x.Key) ? vocation.Skill[(byte)x.Key] : 1, (ushort)x.Value.Level, x.Value.Count));
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
                container.AddItem(BuildContainer(itemModel.Items?.Reverse().ToList(), 0, location, childrenContainer));
            }
            else
            {
                container.AddItem(item);

            }
            return BuildContainer(items, ++index, location, container);
        }

    }
}
