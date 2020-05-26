using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Enums.Location;
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
            new object[] {Slot.Ammo, ItemTestData.CreateAmmoItem(100, 10)},
            new object[] {Slot.Head, ItemTestData.CreateBodyEquipmentItem(100,"head") },
            new object[] {Slot.Left, ItemTestData.CreateWeaponItem(100,"axe")},
            new object[] {Slot.Body, ItemTestData.CreateBodyEquipmentItem(100, "body") },
            new object[] {Slot.Feet, ItemTestData.CreateBodyEquipmentItem(100, "feet") },
            new object[] {Slot.Right, ItemTestData.CreateBodyEquipmentItem(100, "","shield") },
            new object[] {Slot.Ring, ItemTestData.CreateBodyEquipmentItem(100,"ring")},
            new object[] {Slot.Left, ItemTestData.CreateWeaponItem(100,"sword",true)},
            new object[] {Slot.Necklace, ItemTestData.CreateBodyEquipmentItem(100,"necklace")},

       };

        private static List<object[]> GenerateWrongSlotItemsData()
        {
            var result = new List<object[]>();
            foreach (var slot in new List<Slot>() { Slot.Head, Slot.Ammo, Slot.Backpack, Slot.Body, Slot.Feet, Slot.Left, Slot.Right, Slot.Ring, Slot.TwoHanded,
                Slot.Legs, Slot.Necklace })
            {
                if (slot != Slot.Body)
                    result.Add(new object[] { slot, ItemTestData.CreateBodyEquipmentItem(100, "body") });

                if (slot != Slot.Ammo)
                    result.Add(new object[] { slot, ItemTestData.CreateAmmoItem(100, 10) });

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

        public void AddItemToSlot_AddItem_When_Slot_Is_Empty(Slot slot, IPickupable item)
        {
            var sut = new PlayerInventory(PlayerTestDataBuilder.BuildPlayer(), new Dictionary<Slot, Tuple<IPickupable, ushort>>()
            {

            });
            sut.TryAddItemToSlot(slot, item);

            Assert.Same(item, sut[slot]);
        }

        [Theory]
        [MemberData(nameof(WrongSlotItemsData))]

        public void AddItemToSlot_AddItem_On_Wrong_Slot_Returns_False(Slot slot, IPickupable item)
        {
            var sut = new PlayerInventory(PlayerTestDataBuilder.BuildPlayer(), new Dictionary<Slot, Tuple<IPickupable, ushort>>()
            {

            });
            var result = sut.TryAddItemToSlot(slot, item);

            Assert.False(result.Success);
            Assert.Equal(InvalidOperation.CannotDress, result.Error);
        }

        [Fact]
        public void AddItemToSlot_AddTwoHanded_And_Shield_Returns_False()
        {
            var sut = new PlayerInventory(PlayerTestDataBuilder.BuildPlayer(), new Dictionary<Slot, Tuple<IPickupable, ushort>>()
            {

            });

            var twoHanded = ItemTestData.CreateWeaponItem(100, "axe", true);
            var result = sut.TryAddItemToSlot(Slot.Left, twoHanded);

            Assert.Same(twoHanded, sut[Slot.Left]);
            Assert.Null(sut[Slot.Right]);
            Assert.True(result.Success);

            var shield = ItemTestData.CreateBodyEquipmentItem(101, "", "shield");
            Assert.Same(twoHanded, sut[Slot.Left]);

            result = sut.TryAddItemToSlot(Slot.Right, shield);
            Assert.False(result.Success);
            Assert.Equal(InvalidOperation.BothHandsNeedToBeFree, result.Error);

            Assert.Null(sut[Slot.Right]);
        }
        [Fact]
        public void AddItemToSlot_Add_Shield_And_TwoHanded_Returns_False()
        {
            var sut = new PlayerInventory(PlayerTestDataBuilder.BuildPlayer(), new Dictionary<Slot, Tuple<IPickupable, ushort>>()
            {

            });

            var shield = ItemTestData.CreateBodyEquipmentItem(101, "", "shield");
            
            var result = sut.TryAddItemToSlot(Slot.Right, shield);

            Assert.Same(shield, sut[Slot.Right]);
            Assert.Null(sut[Slot.Left]);
            Assert.True(result.Success);

            var twoHanded = ItemTestData.CreateWeaponItem(100, "axe", true);
            Assert.Same(shield, sut[Slot.Right]);

            result = sut.TryAddItemToSlot(Slot.Left, twoHanded);
            Assert.False(result.Success);
            Assert.Equal(InvalidOperation.BothHandsNeedToBeFree, result.Error);

            Assert.Null(sut[Slot.Left]);
        }

        [Fact]
        public void AddItemToSlot_When_Exceeds_Capacity_Returns_False()
        {
            var sut = new PlayerInventory(PlayerTestDataBuilder.BuildPlayer(capacity: 100), new Dictionary<Slot, Tuple<IPickupable, ushort>>()
            {

            });

            var legs = ItemTestData.CreateBodyEquipmentItem(100, "legs");
            var feet = ItemTestData.CreateBodyEquipmentItem(101, "feet");
            var body = ItemTestData.CreateBodyEquipmentItem(100, "body");

            sut.TryAddItemToSlot(Slot.Legs, legs);
            var result = sut.TryAddItemToSlot(Slot.Feet, feet);

            Assert.True(result.Success);

            result = sut.TryAddItemToSlot(Slot.Body, body);

            Assert.False(result.Success);
            Assert.Equal(InvalidOperation.TooHeavy, result.Error);

        }
        [Fact]
        public void AddItemToBackpack_When_Exceeds_Capacity_Returns_False()
        {
            var sut = new PlayerInventory(PlayerTestDataBuilder.BuildPlayer(capacity: 130), new Dictionary<Slot, Tuple<IPickupable, ushort>>()
            {

            });

            var legs = ItemTestData.CreateBodyEquipmentItem(100, "legs");
            var body = ItemTestData.CreateBodyEquipmentItem(100, "body");

            sut.TryAddItemToSlot(Slot.Legs, legs);
            sut.TryAddItemToSlot(Slot.Body, body);

            var bp = ItemTestData.CreateBackpack();
            var bp2 = ItemTestData.CreateBackpack();

            bp.TryAddItem(bp2);

            var result = sut.TryAddItemToSlot(Slot.Backpack, bp);

            Assert.True(result.Success);

            result = sut.TryAddItemToSlot(Slot.Backpack, ItemTestData.CreateAmmoItem(105, 20));

            Assert.False(result.Success);
            Assert.Equal(InvalidOperation.TooHeavy, result.Error);
        }

        [Fact]
        public void AddItemToBackpack_When_Already_Has_Backpack_Add_Item_To_It()
        {
            var sut = new PlayerInventory(PlayerTestDataBuilder.BuildPlayer(capacity: 200), new Dictionary<Slot, Tuple<IPickupable, ushort>>()
            {

            });

            var legs = ItemTestData.CreateBodyEquipmentItem(100, "legs");
            var body = ItemTestData.CreateBodyEquipmentItem(100, "body");

            sut.TryAddItemToSlot(Slot.Legs, legs);
            sut.TryAddItemToSlot(Slot.Body, body);

            var bp = ItemTestData.CreateBackpack();
            var bp2 = ItemTestData.CreateBackpack();

            bp.TryAddItem(bp2);

            var result = sut.TryAddItemToSlot(Slot.Backpack, bp);

            Assert.True(result.Success);

            result = sut.TryAddItemToSlot(Slot.Backpack, ItemTestData.CreateAmmoItem(105, 20));

            Assert.Equal(105, (sut[Slot.Backpack] as IPickupableContainer)[0].ClientId);
        }

        [Fact]
        public void TotalWeight_Returns_Total_Weight_Of_Inventory()
        {
            var sut = new PlayerInventory(PlayerTestDataBuilder.BuildPlayer(capacity: 2000), new Dictionary<Slot, Tuple<IPickupable, ushort>>()
            {

            });

            var legs = ItemTestData.CreateBodyEquipmentItem(100, "legs");
            var body = ItemTestData.CreateBodyEquipmentItem(100, "body");
            var feet = ItemTestData.CreateBodyEquipmentItem(100, "feet");
            var head = ItemTestData.CreateBodyEquipmentItem(100, "head");
            var necklace = ItemTestData.CreateBodyEquipmentItem(100, "necklace");
            var ring = ItemTestData.CreateBodyEquipmentItem(100, "ring");
            var shield = ItemTestData.CreateBodyEquipmentItem(100, "", "shield");
            var ammo = ItemTestData.CreateAmmoItem(100, 100);
            var weapon = ItemTestData.CreateWeaponItem(100, "club", false);

            sut.TryAddItemToSlot(Slot.Legs, legs);
            sut.TryAddItemToSlot(Slot.Body, body);
            sut.TryAddItemToSlot(Slot.Feet, feet);
            sut.TryAddItemToSlot(Slot.Head, head);
            sut.TryAddItemToSlot(Slot.Necklace, necklace);
            sut.TryAddItemToSlot(Slot.Ring, ring);
            sut.TryAddItemToSlot(Slot.Right, shield);
            sut.TryAddItemToSlot(Slot.Left, weapon);
            sut.TryAddItemToSlot(Slot.Ammo, ammo);

            var container = ItemTestData.CreateBackpack();
            sut.TryAddItemToSlot(Slot.Backpack, container);

            container.TryAddItem(ItemTestData.CreateCumulativeItem(100,60));

            Assert.Equal(480, sut.TotalWeight);

        }

    }
}
