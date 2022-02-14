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
using NeoServer.Game.Creatures.Model;
using NeoServer.Game.Creatures.Model.Players;
using NeoServer.Game.Creatures.Vocations;
using NeoServer.Game.World.Services;
using PathFinder = NeoServer.Game.World.Map.PathFinder;

namespace NeoServer.Game.Tests.Helpers;

public static class PlayerTestDataBuilder
{
    public static IPlayer Build(uint id = 1, string name = "PlayerA", uint capacity = 100, ushort hp = 100,
        ushort mana = 30, ushort speed = 200,
        Dictionary<Slot, Tuple<IPickupable, ushort>> inventoryMap = null,
        Dictionary<SkillType, ISkill> skills = null,
        byte vocationType = 1, IPathFinder pathFinder = null, IWalkToMechanism walkToMechanism = null,
        IVocationStore vocationStore = null, IGuild guild = null)
    {
        var vocation = new Vocation
        {
            Id = vocationType.ToString(),
            Name = "Knight"
        };

        if (vocationStore is null)
        {
            vocationStore = new VocationStore();
            vocationStore.Add(vocationType, vocation);
        }

        var map = MapTestDataBuilder.Build(100, 110, 100, 110, 7, 7, true);
        pathFinder ??= new PathFinder(map);
        var mapTool = new MapTool(map, pathFinder);

        var player = new Player(id, name, ChaseMode.Stand, capacity, hp, hp, vocationStore.Get(vocationType),
            Gender.Male, true, mana,
            mana,
            FightMode.Attack,
            100, 100,
            skills ?? new Dictionary<SkillType, ISkill>
                { { SkillType.Level, new Skill(SkillType.Level, 10, 1) } },
            300, new Outfit(), speed, new Location(100, 100, 7), mapTool, walkToMechanism
        )
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

    public static Dictionary<SkillType, ISkill> GenerateSkills(ushort level)
    {
        return new()
        {
            [SkillType.Axe] = new Skill(SkillType.Axe, level),
            [SkillType.Sword] = new Skill(SkillType.Sword, level),
            [SkillType.Club] = new Skill(SkillType.Club, level),
            [SkillType.Distance] = new Skill(SkillType.Distance, level),
            [SkillType.Fishing] = new Skill(SkillType.Fishing, level),
            [SkillType.Fist] = new Skill(SkillType.Fist, level),
            [SkillType.Level] = new Skill(SkillType.Level, level),
            [SkillType.Magic] = new Skill(SkillType.Magic, level),
            [SkillType.Shielding] = new Skill(SkillType.Shielding, level),
            [SkillType.Speed] = new Skill(SkillType.Speed, level)
        };
    }

    public static Dictionary<Slot, Tuple<IPickupable, ushort>> GenerateInventory()
    {
        return new()
        {
            [Slot.Backpack] = new Tuple<IPickupable, ushort>(ItemTestData.CreateBackpack(), 1),
            [Slot.Ammo] = new Tuple<IPickupable, ushort>(ItemTestData.CreateAmmo(2, 10), 2),
            [Slot.Head] = new Tuple<IPickupable, ushort>(ItemTestData.CreateBodyEquipmentItem(3, "head"), 3),
            [Slot.Left] = new Tuple<IPickupable, ushort>(ItemTestData.CreateWeaponItem(4, "axe"), 4),
            [Slot.Body] = new Tuple<IPickupable, ushort>(ItemTestData.CreateBodyEquipmentItem(5, "body"), 5),
            [Slot.Feet] = new Tuple<IPickupable, ushort>(ItemTestData.CreateBodyEquipmentItem(6, "feet"), 6),
            [Slot.Right] = new Tuple<IPickupable, ushort>(ItemTestData.CreateBodyEquipmentItem(7, "", "shield"), 7),
            [Slot.Ring] =
                new Tuple<IPickupable, ushort>(ItemTestData.CreateDefenseEquipmentItem(8, "ring"), 8),
            [Slot.Necklace] =
                new Tuple<IPickupable, ushort>(ItemTestData.CreateDefenseEquipmentItem(10, "necklace"),
                    10),
            [Slot.Legs] = new Tuple<IPickupable, ushort>(ItemTestData.CreateBodyEquipmentItem(11, "legs"), 11)
        };
    }
}