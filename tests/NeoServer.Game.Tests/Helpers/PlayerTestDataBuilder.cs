using System;
using System.Collections.Generic;
using NeoServer.Data.InMemory.DataStores;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Parsers;
using NeoServer.Game.Creatures.Model;
using NeoServer.Game.Creatures.Model.Players;
using NeoServer.Game.Creatures.Vocations;

namespace NeoServer.Game.Tests.Helpers
{
    public static class PlayerTestDataBuilder
    {
        public static IPlayer Build(uint id = 1, string name = "PlayerA", uint capacity = 100, ushort hp = 100,
            ushort mana = 30, ushort speed = 200,
            Dictionary<Slot, Tuple<IPickupable, ushort>> inventoryMap = null, Dictionary<SkillType, ISkill> skills = null,
            byte vocationType = 1, IPathFinder pathFinder = null, IWalkToMechanism walkToMechanism = null,
            IVocationStore vocationStore = null, IGuild guild= null)
        {
            var vocation = new Vocation()
            {
                Id = vocationType.ToString(),
                Name = "Knight",
            };

            if (vocationStore is null)
            {
                vocationStore = new VocationStore();
                vocationStore.Add(vocationType, vocation);
            }

            var player = new Player(id, name, ChaseMode.Stand, capacity, hp, hp, vocationStore.Get(vocationType), Gender.Male, true, mana,
                mana,
                FightMode.Attack,
                100, 100,
                skills ?? new Dictionary<SkillType, ISkill>
                    { { SkillType.Level, new Skill(SkillType.Level, 1, 10, 1) } },
                300, new Outfit(), speed, new Location(100, 100, 7), pathFinder, walkToMechanism)
            {
                Guild = guild
            };

            if (inventoryMap is not null)
            {
                var inventory = InventoryTestDataBuilder.Build(player, inventoryMap);
                player.AddInventory(inventory);
            }

            return player;
        }

        public static Dictionary<SkillType, ISkill> GenerateSkills(ushort level) =>
            new()
            {
                [SkillType.Axe] = new Skill(SkillType.Axe, 1, level, 0),
                [SkillType.Sword] = new Skill(SkillType.Sword, 1, level, 0),
                [SkillType.Club] = new Skill(SkillType.Club, 1, level, 0),
                [SkillType.Distance] = new Skill(SkillType.Distance, 1, level, 0),
                [SkillType.Fishing] = new Skill(SkillType.Fishing, 1, level, 0),
                [SkillType.Fist] = new Skill(SkillType.Fist, 1, level, 0),
                [SkillType.Level] = new Skill(SkillType.Level, 1, level, 0),
                [SkillType.Magic] = new Skill(SkillType.Magic, 1, level, 0),
                [SkillType.Shielding] = new Skill(SkillType.Shielding, 1, level, 0),
                [SkillType.Speed] = new Skill(SkillType.Speed, 1, level, 0)
            };

        public static Dictionary<Slot, Tuple<IPickupable, ushort>> GenerateInventory() =>
            new()
            {
                [Slot.Backpack] = new Tuple<IPickupable, ushort>(ItemTestData.CreateBackpack(), 1),
                [Slot.Ammo] = new Tuple<IPickupable, ushort>(ItemTestData.CreateAmmo(2, 10), 2),
                [Slot.Head] = new Tuple<IPickupable, ushort>(ItemTestData.CreateBodyEquipmentItem(3, "head"), 3),
                [Slot.Left] = new Tuple<IPickupable, ushort>(ItemTestData.CreateWeaponItem(4, "axe"), 4),
                [Slot.Body] = new Tuple<IPickupable, ushort>(ItemTestData.CreateBodyEquipmentItem(5, "body"), 5),
                [Slot.Feet] = new Tuple<IPickupable, ushort>(ItemTestData.CreateBodyEquipmentItem(6, "feet"), 6),
                [Slot.Right] = new Tuple<IPickupable, ushort>(ItemTestData.CreateBodyEquipmentItem(7, "", "shield"), 7),
                [Slot.Ring] =
                    new Tuple<IPickupable, ushort>(ItemTestData.CreateDefenseEquipmentItem(id: 8, slot: "ring"), 8),
                [Slot.Necklace] =
                    new Tuple<IPickupable, ushort>(ItemTestData.CreateDefenseEquipmentItem(id: 10, slot: "necklace"),
                        10),
                [Slot.Legs] = new Tuple<IPickupable, ushort>(ItemTestData.CreateBodyEquipmentItem(11, "legs"), 11)
            };
    }
}