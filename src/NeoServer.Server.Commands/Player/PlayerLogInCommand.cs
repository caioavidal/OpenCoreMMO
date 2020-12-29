using NeoServer.Data.Model;
using NeoServer.Game.Contracts;
using NeoServer.Server.Contracts.Network;
using System.Linq;
using System.Threading.Tasks;
using NeoServer.Server.Model.Players;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures;
using NeoServer.Game.Creature.Model;
using System.Collections.Generic;
using NeoServer.Game.Common.Players;
using NeoServer.Game.Contracts.Items.Types;
using System;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Common;

namespace NeoServer.Server.Commands
{
    public class PlayerLogInCommand : Command
    {
        private readonly AccountModel account;
        private readonly string characterName;
        private readonly Game game;
        private readonly IConnection connection;
        private CreaturePathAccess _creaturePathAccess;
        private readonly IItemFactory _itemFactory;

        public PlayerLogInCommand(AccountModel account, string characterName, Game game, IConnection connection, CreaturePathAccess creaturePathAccess, IItemFactory itemFactory)
        {
            this.account = account;
            this.characterName = characterName;

            this.game = game;
            this.connection = connection;
            _creaturePathAccess = creaturePathAccess;
            _itemFactory = itemFactory;
        }

        public override void Execute()
        {
            var playerRecord = account.Players.FirstOrDefault(p => p.Name.Equals(characterName));

            if (playerRecord == null)
            {
                //todo validations here
                return;
            }

            //MOVE TO CORRECT LOCAL AND CREATE LOADER

            var location = new Location((ushort)playerRecord.PosX, (ushort)playerRecord.PosY, (byte)playerRecord.PosZ);

            var outfit = new Outfit
            {
                LookType = 75
            };


            var iventory = new Dictionary<Slot, Tuple<IPickupable, ushort>>();

            var skills = new Dictionary<SkillType, ISkill>();

            skills.Add(SkillType.Axe, new Skill(SkillType.Axe, 1, (ushort)playerRecord.SkillAxe));
            skills.Add(SkillType.Club, new Skill(SkillType.Club, 1, (ushort)playerRecord.SkillClub));
            skills.Add(SkillType.Distance, new Skill(SkillType.Distance, 1, (ushort)playerRecord.SkillDist));
            skills.Add(SkillType.Fishing, new Skill(SkillType.Fishing, 1, (ushort)playerRecord.SkillFishing));
            skills.Add(SkillType.Fist, new Skill(SkillType.Fist, 1, (ushort)playerRecord.SkillFist));
            skills.Add(SkillType.Level, new Skill(SkillType.Level, 1, (ushort)playerRecord.Level));
            skills.Add(SkillType.Magic, new Skill(SkillType.Magic, 1, (ushort)playerRecord.Mana));
            skills.Add(SkillType.Shielding, new Skill(SkillType.Shielding, 1, (ushort)playerRecord.SkillShielding));
            skills.Add(SkillType.Speed, new Skill(SkillType.Speed, 1, (ushort)playerRecord.Speed));
            skills.Add(SkillType.Sword, new Skill(SkillType.Sword, 1, (ushort)playerRecord.SkillSword));

            foreach (var item in playerRecord.PlayerItems.Where(c => c.Pid <= 10))
            {
                //var item2 = _itemFactory.Create((ushort)item.Sid, location, new Dictionary<ItemAttribute, IConvertible>()
                //{
                //    {ItemAttribute.Count, item.Count }
                //});

                if (!(_itemFactory.Create((ushort)item.Itemtype, location, null) is IPickupable createdItem))
                {
                    continue;
                }

                //if (slot.Key == Slot.Backpack)
                if (item.Pid == 3)
                {
                    if (createdItem is not IContainer container) continue;
                    BuildContainer(playerRecord.PlayerItems.Where(c => c.Pid.Equals(item.Sid)).ToList(), 0, location, container, playerRecord.PlayerItems.ToList());
                }

                if (item.Pid == 1)
                    iventory.Add(Slot.Necklace, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.Itemtype));
                else if (item.Pid == 2)
                    iventory.Add(Slot.Head, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.Itemtype));
                else if (item.Pid == 3)
                    iventory.Add(Slot.Backpack, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.Itemtype));
                else if (item.Pid == 4)
                    iventory.Add(Slot.Left, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.Itemtype));
                else if (item.Pid == 5)
                    iventory.Add(Slot.Body, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.Itemtype));
                else if (item.Pid == 6)
                    iventory.Add(Slot.Right, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.Itemtype));
                else if (item.Pid == 7)
                    iventory.Add(Slot.Ring, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.Itemtype));
                else if (item.Pid == 8)
                    iventory.Add(Slot.Legs, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.Itemtype));
                else if (item.Pid == 9)
                    iventory.Add(Slot.Ammo, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.Itemtype));
                else if (item.Pid == 10)
                    iventory.Add(Slot.Feet, new Tuple<IPickupable, ushort>(createdItem, (ushort)item.Itemtype));
            }

            var newPlayer = new Model.Players.Player(
                (uint)playerRecord.PlayerId,
                playerRecord.Name,
                playerRecord.ChaseMode,
                playerRecord.Capacity,
                playerRecord.Health,
                playerRecord.MaxHealth,
                playerRecord.Vocation,
                playerRecord.Gender,
                playerRecord.Online,
                playerRecord.Mana,
                playerRecord.MaxMana,
                playerRecord.FightMode,
                playerRecord.Soul,
                playerRecord.MaxSoul,
                skills, //ConvertToSkills(player),
                playerRecord.StaminaMinutes,
                outfit, //player.Outfit,
                iventory, //ConvertToInventory(player),
                playerRecord.Speed,
                location,
                _creaturePathAccess
                );

            game.CreatureManager.AddPlayer(newPlayer, connection);
            //game.CreatureManager.AddPlayer(playerRecord, connection);
        }

        public IContainer BuildContainer(List<PlayerItemModel> items, int index, Location location, IContainer container, List<PlayerItemModel> all)
        {
            if (items == null || items.Count == index)
            {
                return container;
            }

            var itemModel = items[index];

            var item = _itemFactory.Create((ushort)itemModel.Itemtype, location, new Dictionary<ItemAttribute, IConvertible>()
                        {
                            {ItemAttribute.Count, itemModel.Count }
                        });

            if (item is IContainer childrenContainer)
            {
                childrenContainer.SetParent(container);
                container.AddItem(BuildContainer(all.Where(c => c.Pid.Equals(itemModel.Sid)).ToList(), 0, location, childrenContainer, all));
            }
            else
            {
                container.AddItem(item);

            }
            return BuildContainer(items, ++index, location, container, all);
        }
    }
}