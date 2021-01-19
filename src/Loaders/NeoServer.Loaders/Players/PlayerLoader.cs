using NeoServer.Data.Model;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Players;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Creature.Model;
using NeoServer.Game.Creatures;
using NeoServer.Game.Creatures.Vocations;
using NeoServer.Loaders.Interfaces;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Loaders.Players
{
    public class PlayerLoader: IPlayerLoader
    {
        private CreaturePathAccess _creaturePathAccess;
        private IItemFactory itemFactory;
        private readonly ICreatureFactory creatureFactory;

        public virtual bool IsApplicable(PlayerModel player) => player.PlayerType == 1;

        public PlayerLoader(CreaturePathAccess creaturePathAccess, IItemFactory itemFactory, ICreatureFactory creatureFactory)
        {
            _creaturePathAccess = creaturePathAccess;
            this.itemFactory = itemFactory;
            this.creatureFactory = creatureFactory;
        }
        public virtual IPlayer Load(PlayerModel player)
        {
            if (!VocationStore.TryGetValue(player.Vocation, out var vocation))
            {
                throw new Exception("Player vocation not found");
            }

            var newPlayer = new Player(
                (uint)player.PlayerId,
                player.Name,
                player.ChaseMode,
                player.Capacity,
                player.Health,
                player.MaxHealth,
                player.Vocation,
                player.Gender,
                player.Online,
                player.Mana,
                player.MaxMana,
                player.FightMode,
                player.Soul,
                vocation.SoulMax,
                ConvertToSkills(player),
                player.StaminaMinutes,
                new Outfit() { Addon = (byte) player.LookAddons, Body = (byte)player.LookBody, Feet  = (byte) player.LookFeet,  Head = (byte)player.LookHead, Legs = (byte) player.LookLegs, LookType = (byte) player.LookType}, 
                ConvertToInventory(player),
                player.Speed,
                new Location((ushort)player.PosX, (ushort)player.PosY, (byte)player.PosZ),
               _creaturePathAccess
                );

            return creatureFactory.CreatePlayer(newPlayer);
        }

        protected Dictionary<SkillType, ISkill> ConvertToSkills(PlayerModel playerRecord)
        {
            VocationStore.TryGetValue(playerRecord.Vocation, out var vocation);

            Func<SkillType, float> skillRate = (skill) => vocation.Skill?.ContainsKey((byte)skill) ?? false ? vocation.Skill[(byte)skill] : 1;

            var skills = new Dictionary<SkillType, ISkill>();

            skills.Add(SkillType.Axe, new Skill(SkillType.Axe, skillRate(SkillType.Axe), (ushort)playerRecord.SkillAxe, playerRecord.SkillAxeTries));
            skills.Add(SkillType.Club, new Skill(SkillType.Club, skillRate(SkillType.Club), (ushort)playerRecord.SkillClub, playerRecord.SkillClubTries));
            skills.Add(SkillType.Distance, new Skill(SkillType.Distance, skillRate(SkillType.Distance), (ushort)playerRecord.SkillDist, playerRecord.SkillDistTries));
            skills.Add(SkillType.Fishing, new Skill(SkillType.Fishing, skillRate(SkillType.Fishing), (ushort)playerRecord.SkillFishing, playerRecord.SkillFishingTries));
            skills.Add(SkillType.Fist, new Skill(SkillType.Fist, skillRate(SkillType.Fist), (ushort)playerRecord.SkillFist, playerRecord.SkillFistTries));
            skills.Add(SkillType.Shielding, new Skill(SkillType.Shielding, skillRate(SkillType.Shielding), (ushort)playerRecord.SkillShielding, playerRecord.SkillShieldingTries));

            skills.Add(SkillType.Level, new Skill(SkillType.Level, skillRate(SkillType.Level), playerRecord.Level, playerRecord.Experience));
            skills.Add(SkillType.Magic, new Skill(SkillType.Magic, skillRate(SkillType.Magic), (ushort)playerRecord.MagicLevel, playerRecord.MagicLevelTries));
            skills.Add(SkillType.Sword, new Skill(SkillType.Sword, skillRate(SkillType.Sword), (ushort)playerRecord.SkillSword, playerRecord.SkillSwordTries));

            return skills;
        }


        protected IDictionary<Slot, Tuple<IPickupable, ushort>> ConvertToInventory(PlayerModel playerRecord)
        {
            var inventory = new Dictionary<Slot, Tuple<IPickupable, ushort>>();
            var attrs = new Dictionary<ItemAttribute, IConvertible>() { { ItemAttribute.Count, 0 } };

            foreach (var item in playerRecord.PlayerInventoryItems)
            {
                attrs[ItemAttribute.Count] = (byte)item.Amount;
                var location = item.SlotId <= 10 ? Location.Inventory((Slot)item.SlotId) : Location.Container(0, 0);

                if (!(itemFactory.Create((ushort)item.ServerId, location, attrs) is IPickupable createdItem))
                {
                    continue;
                }

                if (item.SlotId == (int)Slot.Backpack)
                {
                    if (createdItem is not IContainer container) continue;
                    BuildContainer(playerRecord.PlayerItems.Where(c => c.ParentId.Equals(0)).ToList(), 0, location, container, playerRecord.PlayerItems.ToList());
                }

                if (item.SlotId == (int)Slot.Necklace)
                    inventory.Add(Slot.Necklace, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.ServerId));
                else if (item.SlotId == (int)Slot.Head)
                    inventory.Add(Slot.Head, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.ServerId));
                else if (item.SlotId == (int)Slot.Backpack)
                    inventory.Add(Slot.Backpack, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.ServerId));
                else if (item.SlotId == (int)Slot.Left)
                    inventory.Add(Slot.Left, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.ServerId));
                else if (item.SlotId == (int)Slot.Body)
                    inventory.Add(Slot.Body, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.ServerId));
                else if (item.SlotId == (int)Slot.Right)
                    inventory.Add(Slot.Right, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.ServerId));
                else if (item.SlotId == (int)Slot.Ring)
                    inventory.Add(Slot.Ring, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.ServerId));
                else if (item.SlotId == (int)Slot.Legs)
                    inventory.Add(Slot.Legs, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.ServerId));
                else if (item.SlotId == (int)Slot.Ammo)
                    inventory.Add(Slot.Ammo, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.ServerId));
                else if (item.SlotId == (int)Slot.Feet)
                    inventory.Add(Slot.Feet, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.ServerId));
            }
            return inventory;
        }

        private IContainer BuildContainer(List<PlayerItemModel> items, int index, Location location, IContainer container, List<PlayerItemModel> all)
        {
            if (items == null || items.Count == index)
            {
                return container;
            }

            var itemModel = items[index];

            var item = itemFactory.Create((ushort)itemModel.ServerId, location, new Dictionary<ItemAttribute, IConvertible>()
                        {
                            {ItemAttribute.Count, (byte) itemModel.Amount }
                        });

            if (item is IContainer childrenContainer)
            {
                childrenContainer.SetParent(container);
                container.AddItem(BuildContainer(all.Where(c => c.ParentId.Equals(itemModel.Id)).ToList(), 0, location, childrenContainer, all));
            }
            else
            {
                container.AddItem(item);

            }
            return BuildContainer(items, ++index, location, container, all);
        }
    }
}
