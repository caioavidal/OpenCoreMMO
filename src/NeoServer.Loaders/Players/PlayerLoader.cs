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
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Loaders.Players
{
    public class PlayerLoader
    {
        private CreaturePathAccess _creaturePathAccess;
        private IItemFactory itemFactory;
        private readonly ICreatureFactory creatureFactory;

        public PlayerLoader(CreaturePathAccess creaturePathAccess, IItemFactory itemFactory, ICreatureFactory creatureFactory)
        {
            _creaturePathAccess = creaturePathAccess;
            this.itemFactory = itemFactory;
            this.creatureFactory = creatureFactory;
        }
        public IPlayer Load(PlayerModel player)
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

        private Dictionary<SkillType, ISkill> ConvertToSkills(PlayerModel playerRecord)
        {
            VocationStore.TryGetValue(playerRecord.Vocation, out var vocation);

            Func<SkillType, float> skillRate = (skill) => vocation.Skill.ContainsKey((byte)skill) ? vocation.Skill[(byte)skill] : 1;

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


        private IDictionary<Slot, Tuple<IPickupable, ushort>> ConvertToInventory(PlayerModel playerRecord)
        {
            var inventory = new Dictionary<Slot, Tuple<IPickupable, ushort>>();

            foreach (var item in playerRecord.PlayerItems)
            {
                var location = item.ParentId <= 10 ? Location.Inventory((Slot)item.ParentId) : Location.Container(0, 0);

                if (!(itemFactory.Create((ushort)item.Itemtype, location, null) is IPickupable createdItem))
                {
                    continue;
                }

                if (item.ParentId == (int)Slot.Backpack)
                {
                    if (createdItem is not IContainer container) continue;
                    BuildContainer(playerRecord.PlayerItems.Where(c => c.ParentId.Equals(item.ServerId)).ToList(), 0, location, container, playerRecord.PlayerItems.ToList());
                }


                if (item.ParentId == 1)
                    inventory.Add(Slot.Necklace, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.Itemtype));
                else if (item.ParentId == 2)
                    inventory.Add(Slot.Head, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.Itemtype));
                else if (item.ParentId == 3)
                    inventory.Add(Slot.Backpack, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.Itemtype));
                else if (item.ParentId == 4)
                    inventory.Add(Slot.Left, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.Itemtype));
                else if (item.ParentId == 5)
                    inventory.Add(Slot.Body, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.Itemtype));
                else if (item.ParentId == 6)
                    inventory.Add(Slot.Right, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.Itemtype));
                else if (item.ParentId == 7)
                    inventory.Add(Slot.Ring, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.Itemtype));
                else if (item.ParentId == 8)
                    inventory.Add(Slot.Legs, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.Itemtype));
                else if (item.ParentId == 9)
                    inventory.Add(Slot.Ammo, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.Itemtype));
                else if (item.ParentId == 10)
                    inventory.Add(Slot.Feet, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.Itemtype));


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

            var item = itemFactory.Create((ushort)itemModel.Itemtype, location, new Dictionary<ItemAttribute, IConvertible>()
                        {
                            {ItemAttribute.Count, itemModel.Amount }
                        });

            if (item is IContainer childrenContainer)
            {
                childrenContainer.SetParent(container);
                container.AddItem(BuildContainer(all.Where(c => c.ParentId.Equals(itemModel.ServerId)).ToList(), 0, location, childrenContainer, all));
            }
            else
            {
                container.AddItem(item);

            }
            return BuildContainer(items, ++index, location, container, all);
        }
    }
}
