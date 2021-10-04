using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Data.Model;
using NeoServer.Game.Chats;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Model;
using NeoServer.Game.Creatures.Model.Players;
using NeoServer.Game.Creatures.Vocations;
using NeoServer.Game.DataStore;
using NeoServer.Loaders.Interfaces;

namespace NeoServer.Loaders.Players
{
    public class PlayerLoader : IPlayerLoader
    {
        private readonly ChatChannelFactory _chatChannelFactory;
        private readonly ICreatureFactory _creatureFactory;
        private readonly IItemFactory _itemFactory;

        public PlayerLoader(IItemFactory itemFactory, ICreatureFactory creatureFactory,
            ChatChannelFactory chatChannelFactory)
        {
            _itemFactory = itemFactory;
            _creatureFactory = creatureFactory;
            _chatChannelFactory = chatChannelFactory;
        }

        public virtual bool IsApplicable(PlayerModel player)
        {
            return player.PlayerType == 1;
        }

        public virtual IPlayer Load(PlayerModel playerModel)
        {
            if (!VocationStore.Data.TryGetValue(playerModel.Vocation, out var vocation))
                throw new Exception("Player vocation not found");

            var player = new Player(
                (uint) playerModel.PlayerId,
                playerModel.Name,
                playerModel.ChaseMode,
                playerModel.Capacity,
                playerModel.Health,
                playerModel.MaxHealth,
                playerModel.Vocation,
                playerModel.Gender,
                playerModel.Online,
                playerModel.Mana,
                playerModel.MaxMana,
                playerModel.FightMode,
                playerModel.Soul,
                vocation.SoulMax,
                ConvertToSkills(playerModel),
                playerModel.StaminaMinutes,
                new Outfit
                {
                    Addon = (byte) playerModel.LookAddons, Body = (byte) playerModel.LookBody,
                    Feet = (byte) playerModel.LookFeet, Head = (byte) playerModel.LookHead,
                    Legs = (byte) playerModel.LookLegs, LookType = (byte) playerModel.LookType
                },
                ConvertToInventory(playerModel),
                0,
                new Location((ushort) playerModel.PosX, (ushort) playerModel.PosY, (byte) playerModel.PosZ)
            )
            {
                AccountId = (uint) playerModel.AccountId,
                GuildId = (ushort) (playerModel?.GuildMember?.GuildId ?? 0),
                GuildLevel = (ushort) (playerModel?.GuildMember?.RankId ?? 0)
            };

            AddExistingPersonalChannels(player);

            return _creatureFactory.CreatePlayer(player);
        }

        /// <summary>
        ///     Adds all PersonalChatChannel assemblies to Player
        /// </summary>
        public virtual void AddExistingPersonalChannels(IPlayer player)
        {
            if (player is null) return;

            var personalChannels = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => typeof(PersonalChatChannel).IsAssignableFrom(x));
            foreach (var channel in personalChannels)
            {
                if (channel == typeof(PersonalChatChannel)) continue;

                var createdChannel = _chatChannelFactory.Create(channel, null, player);
                player.AddPersonalChannel(createdChannel);
            }
        }

        protected Dictionary<SkillType, ISkill> ConvertToSkills(PlayerModel playerRecord)
        {
            VocationStore.Data.TryGetValue(playerRecord.Vocation, out var vocation);

            Func<SkillType, float> skillRate = skill =>
                vocation.Skill?.ContainsKey((byte) skill) ?? false ? vocation.Skill[(byte) skill] : 1;

            var skills = new Dictionary<SkillType, ISkill>();

            skills.Add(SkillType.Axe,
                new Skill(SkillType.Axe, skillRate(SkillType.Axe), (ushort) playerRecord.SkillAxe,
                    playerRecord.SkillAxeTries));
            skills.Add(SkillType.Club,
                new Skill(SkillType.Club, skillRate(SkillType.Club), (ushort) playerRecord.SkillClub,
                    playerRecord.SkillClubTries));
            skills.Add(SkillType.Distance,
                new Skill(SkillType.Distance, skillRate(SkillType.Distance), (ushort) playerRecord.SkillDist,
                    playerRecord.SkillDistTries));
            skills.Add(SkillType.Fishing,
                new Skill(SkillType.Fishing, skillRate(SkillType.Fishing), (ushort) playerRecord.SkillFishing,
                    playerRecord.SkillFishingTries));
            skills.Add(SkillType.Fist,
                new Skill(SkillType.Fist, skillRate(SkillType.Fist), (ushort) playerRecord.SkillFist,
                    playerRecord.SkillFistTries));
            skills.Add(SkillType.Shielding,
                new Skill(SkillType.Shielding, skillRate(SkillType.Shielding), (ushort) playerRecord.SkillShielding,
                    playerRecord.SkillShieldingTries));

            skills.Add(SkillType.Level,
                new Skill(SkillType.Level, skillRate(SkillType.Level), playerRecord.Level, playerRecord.Experience));
            skills.Add(SkillType.Magic,
                new Skill(SkillType.Magic, skillRate(SkillType.Magic), (ushort) playerRecord.MagicLevel,
                    playerRecord.MagicLevelTries));
            skills.Add(SkillType.Sword,
                new Skill(SkillType.Sword, skillRate(SkillType.Sword), (ushort) playerRecord.SkillSword,
                    playerRecord.SkillSwordTries));

            return skills;
        }

        protected IDictionary<Slot, Tuple<IPickupable, ushort>> ConvertToInventory(PlayerModel playerRecord)
        {
            var inventory = new Dictionary<Slot, Tuple<IPickupable, ushort>>();
            var attrs = new Dictionary<ItemAttribute, IConvertible> {{ItemAttribute.Count, 0}};

            foreach (var item in playerRecord.PlayerInventoryItems)
            {
                attrs[ItemAttribute.Count] = (byte) item.Amount;
                var location = item.SlotId <= 10 ? Location.Inventory((Slot) item.SlotId) : Location.Container(0, 0);

                if (!(_itemFactory.Create((ushort) item.ServerId, location, attrs) is IPickupable createdItem)) continue;

                if (item.SlotId == (int) Slot.Backpack)
                {
                    if (createdItem is not IContainer container) continue;
                    BuildContainer(playerRecord.PlayerItems.Where(c => c.ParentId.Equals(0)).ToList(), 0, location,
                        container, playerRecord.PlayerItems.ToList());
                }

                if (item.SlotId == (int) Slot.Necklace)
                    inventory.Add(Slot.Necklace, new Tuple<IPickupable, ushort>(createdItem, (ushort) item.ServerId));
                else if (item.SlotId == (int) Slot.Head)
                    inventory.Add(Slot.Head, new Tuple<IPickupable, ushort>(createdItem, (ushort) item.ServerId));
                else if (item.SlotId == (int) Slot.Backpack)
                    inventory.Add(Slot.Backpack, new Tuple<IPickupable, ushort>(createdItem, (ushort) item.ServerId));
                else if (item.SlotId == (int) Slot.Left)
                    inventory.Add(Slot.Left, new Tuple<IPickupable, ushort>(createdItem, (ushort) item.ServerId));
                else if (item.SlotId == (int) Slot.Body)
                    inventory.Add(Slot.Body, new Tuple<IPickupable, ushort>(createdItem, (ushort) item.ServerId));
                else if (item.SlotId == (int) Slot.Right)
                    inventory.Add(Slot.Right, new Tuple<IPickupable, ushort>(createdItem, (ushort) item.ServerId));
                else if (item.SlotId == (int) Slot.Ring)
                    inventory.Add(Slot.Ring, new Tuple<IPickupable, ushort>(createdItem, (ushort) item.ServerId));
                else if (item.SlotId == (int) Slot.Legs)
                    inventory.Add(Slot.Legs, new Tuple<IPickupable, ushort>(createdItem, (ushort) item.ServerId));
                else if (item.SlotId == (int) Slot.Ammo)
                    inventory.Add(Slot.Ammo, new Tuple<IPickupable, ushort>(createdItem, (ushort) item.ServerId));
                else if (item.SlotId == (int) Slot.Feet)
                    inventory.Add(Slot.Feet, new Tuple<IPickupable, ushort>(createdItem, (ushort) item.ServerId));
            }

            return inventory;
        }

        private IContainer BuildContainer(List<PlayerItemModel> items, int index, Location location,
            IContainer container, List<PlayerItemModel> all)
        {
            if (items == null || items.Count == index) return container;

            var itemModel = items[index];

            var item = _itemFactory.Create((ushort) itemModel.ServerId, location,
                new Dictionary<ItemAttribute, IConvertible>
                {
                    {ItemAttribute.Count, (byte) itemModel.Amount}
                });

            if (item is IContainer childrenContainer)
            {
                childrenContainer.SetParent(container);
                container.AddItem(BuildContainer(all.Where(c => c.ParentId.Equals(itemModel.Id)).ToList(), 0, location,
                    childrenContainer, all));
            }
            else
            {
                container.AddItem(item);
            }

            return BuildContainer(items, ++index, location, container, all);
        }
    }
}