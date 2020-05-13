using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Enums.Players;
using NeoServer.Game.Items.Items;
using NeoServer.Game.Items.Tests;
using NeoServer.Server.Model.Players;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NeoServer.Game.Creatures.Tests
{
    public class PlayerInventoryTest
    {
        public static IEnumerable<object[]> SlotItemsData =>
       new List<object[]>
       {
            new object[] {Slot.Ammo, ItemTestData.CreatAmmoItem(100, 10)},
            new object[] {Slot.Head, ItemTestData.CreateBodyEquipmentItem(100,"head") },
            new object[] {Slot.Left, ItemTestData.CreateWeaponItem(100,"axe")},
            new object[] {Slot.Body, ItemTestData.CreateBodyEquipmentItem(100, "body") },
            new object[] {Slot.Feet, ItemTestData.CreateBodyEquipmentItem(100, "feet") },
            new object[] {Slot.Right, ItemTestData.CreateBodyEquipmentItem(100, "","shield") },
            new object[] {Slot.Ring, ItemTestData.CreateBodyEquipmentItem(100,"ring")},
            new object[] {Slot.TwoHanded, ItemTestData.CreateWeaponItem(100,"sword",true)},

       };

        private static List<object[]> GenerateWrongSlotItemsData()
        {
            var result = new List<object[]>();
            foreach(var slot in new List<Slot>() { Slot.Head, Slot.Ammo, Slot.Backpack, Slot.Body, Slot.Feet, Slot.Left, Slot.Right, Slot.Ring, Slot.TwoHanded, 
                Slot.Legs, Slot.Necklace })
            {
                if(slot != Slot.Body) 
                    result.Add(new object[] { slot, ItemTestData.CreateBodyEquipmentItem(100, "body") });

                if (slot != Slot.Ammo)
                    result.Add(new object[] { slot, ItemTestData.CreatAmmoItem(100, 10) });

                if (slot != Slot.Legs)
                    result.Add(new object[] { slot, ItemTestData.CreateBodyEquipmentItem(100, "legs") });

                if (slot != Slot.Feet)
                    result.Add(new object[] { slot, ItemTestData.CreateBodyEquipmentItem(100, "feet") });

                if (slot != Slot.Right)
                    result.Add(new object[] { slot, ItemTestData.CreateBodyEquipmentItem(100, "", "shield") });

                if (slot != Slot.Left)
                    result.Add(new object[] { slot, ItemTestData.CreateWeaponItem(100, "axe") });

                if (slot != Slot.Ring)
                    result.Add(new object[] { slot, ItemTestData.CreateRing(100) });

                if (slot != Slot.Necklace)
                    result.Add(new object[] { slot, ItemTestData.CreateNecklace(100) });

                if (slot != Slot.Backpack)
                    result.Add(new object[] { slot, ItemTestData.CreateBackpack() });

                if (slot != Slot.Head)
                    result.Add(new object[] { slot, ItemTestData.CreateBodyEquipmentItem(100, "head") });
            }
            return result;
       
        }

        public static IEnumerable<object[]> WrongSlotItemsData => GenerateWrongSlotItemsData();


        [Theory]
        [MemberData(nameof(SlotItemsData))]

        public void AddItemToSlot_AddItem_When_Slot_Is_Empty(Slot slot, IItem item)
        {
            var sut = new PlayerInventory(PlayerTestDataBuilder.BuildPlayer(), new Dictionary<Slot, Tuple<IItem, ushort>>()
            {

            });
            sut.TryAddItemToSlot(slot, item);

            Assert.Same(item, sut[slot]);
        }

        [Theory]
        [MemberData(nameof(WrongSlotItemsData))]

        public void AddItemToSlot_AddItem_On_Wrong_Slot_Returns_False(Slot slot, IItem item)
        {
            var sut = new PlayerInventory(PlayerTestDataBuilder.BuildPlayer(), new Dictionary<Slot, Tuple<IItem, ushort>>()
            {

            });
            var success = sut.TryAddItemToSlot(slot, item);

            Assert.False(success);
        }

       
    }
}
