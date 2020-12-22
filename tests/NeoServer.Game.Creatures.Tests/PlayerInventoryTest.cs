using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Players;
using NeoServer.Game.Items.Items;
using NeoServer.Game.Items.Tests;
using NeoServer.Server.Model.Players;
using System;
using System.Collections.Generic;
using Xunit;
using NeoServer.Game.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.World.Map.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Creatures.Tests
{
    public class PlayerInventoryTest
    {
        public static IEnumerable<object[]> SlotItemsData =>
       new List<object[]>
       {
           new object[] {Slot.Backpack, ItemTestData.CreateBackpack()},
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

        public static IEnumerable<object[]> BackpackSlotAddItemsData =>
     new List<object[]>
     {
            new object[] {ItemTestData.CreateAmmoItem(100, 10)},
            new object[] {ItemTestData.CreateBodyEquipmentItem(100,"head") },
            new object[] {ItemTestData.CreateWeaponItem(100,"axe")},
            new object[] {ItemTestData.CreateBodyEquipmentItem(100, "body") },
            new object[] {ItemTestData.CreateBodyEquipmentItem(100, "feet") },
            new object[] { ItemTestData.CreateBodyEquipmentItem(100, "","shield") },
            new object[] {ItemTestData.CreateBodyEquipmentItem(100,"ring")},
            new object[] {ItemTestData.CreateWeaponItem(100,"sword",true)},
            new object[] {ItemTestData.CreateBodyEquipmentItem(100,"necklace")},
            new object[] {ItemTestData.CreateCumulativeItem(100,87)},

     };
        public static IEnumerable<object[]> SlotSwapItemsData =>
     new List<object[]>
     {
            new object[] {Slot.Ammo, ItemTestData.CreateAmmoItem(100, 10),ItemTestData.CreateAmmoItem(102, 10)},
            new object[] {Slot.Head, ItemTestData.CreateBodyEquipmentItem(100,"head"),ItemTestData.CreateBodyEquipmentItem(102,"head") },
            new object[] {Slot.Left, ItemTestData.CreateWeaponItem(100,"axe"),ItemTestData.CreateWeaponItem(102,"axe")},
            new object[] {Slot.Body, ItemTestData.CreateBodyEquipmentItem(100, "body"), ItemTestData.CreateBodyEquipmentItem(102, "body")},
            new object[] {Slot.Feet, ItemTestData.CreateBodyEquipmentItem(100, "feet"), ItemTestData.CreateBodyEquipmentItem(102, "feet") },
            new object[] {Slot.Right, ItemTestData.CreateBodyEquipmentItem(100, "","shield"), ItemTestData.CreateBodyEquipmentItem(102, "","shield") },
            new object[] {Slot.Ring, ItemTestData.CreateBodyEquipmentItem(100,"ring"), ItemTestData.CreateBodyEquipmentItem(102,"ring")},
            new object[] {Slot.Left, ItemTestData.CreateWeaponItem(100,"sword",true),ItemTestData.CreateWeaponItem(102,"sword",true)},
            new object[] {Slot.Necklace, ItemTestData.CreateBodyEquipmentItem(100,"necklace"), ItemTestData.CreateBodyEquipmentItem(102,"necklace")},
            //new object[] {Slot.Backpack, ItemTestData.CreateBackpack(), ItemTestData.CreateBackpack()},

     };

        public static IEnumerable<object[]> SlotJoinItemsData =>
     new List<object[]>
     {
                new object[] {Slot.Ammo, ItemTestData.CreateAmmoItem(100, 10),ItemTestData.CreateAmmoItem(100, 10), ItemTestData.CreateAmmoItem(100, 20) },
                new object[] {Slot.Ammo, ItemTestData.CreateAmmoItem(100, 10),ItemTestData.CreateAmmoItem(100, 90), ItemTestData.CreateAmmoItem(100, 100) },
                new object[] {Slot.Ammo, ItemTestData.CreateAmmoItem(100, 50),ItemTestData.CreateAmmoItem(100, 90), ItemTestData.CreateAmmoItem(100, 100) },
                new object[] {Slot.Left, ItemTestData.CreateThrowableDistanceItem(100, 1),ItemTestData.CreateThrowableDistanceItem(100, 5), ItemTestData.CreateThrowableDistanceItem(100, 6) }
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

            Assert.False(result.IsSuccess);
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
            Assert.True(result.IsSuccess);

            var shield = ItemTestData.CreateBodyEquipmentItem(101, "", "shield");
            Assert.Same(twoHanded, sut[Slot.Left]);

            result = sut.TryAddItemToSlot(Slot.Right, shield);
            Assert.False(result.IsSuccess);
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
            Assert.True(result.IsSuccess);

            var twoHanded = ItemTestData.CreateWeaponItem(100, "axe", true);
            Assert.Same(shield, sut[Slot.Right]);

            result = sut.TryAddItemToSlot(Slot.Left, twoHanded);
            Assert.False(result.IsSuccess);
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

            Assert.True(result.IsSuccess);

            result = sut.TryAddItemToSlot(Slot.Body, body);

            Assert.False(result.IsSuccess);
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

            Assert.True(result.IsSuccess);

            result = sut.TryAddItemToSlot(Slot.Backpack, ItemTestData.CreateAmmoItem(105, 20));

            Assert.False(result.IsSuccess);
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

            Assert.True(result.IsSuccess);

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

            container.TryAddItem(ItemTestData.CreateCumulativeItem(100, 60));

            Assert.Equal(500, sut.TotalWeight);

        }

        [Theory]
        [MemberData(nameof(SlotSwapItemsData))]

        public void AddItemToSlot_AddItem_When_Slot_Has_Item_Swap_Item(Slot slot, IPickupable item, IPickupable newItem)
        {
            var sut = new PlayerInventory(PlayerTestDataBuilder.BuildPlayer(), new Dictionary<Slot, Tuple<IPickupable, ushort>>());

            var result = sut.TryAddItemToSlot(slot, item);

            Assert.Null(result.Value);

            result = sut.TryAddItemToSlot(slot, newItem);

            if (item is PickupableContainer)
            {
                Assert.Same(item, sut[slot]);
                Assert.Null(result.Value);
                return;
            }

            Assert.Same(newItem, sut[slot]);
            Assert.Same(item, result.Value);
        }

        [Theory]
        [MemberData(nameof(SlotJoinItemsData))]

        public void AddItemToSlot_When_Slot_Has_Cumulative_Item_Join_Item(Slot slot, ICumulative item, ICumulative newItem, ICumulative resultItem)
        {
            var sut = new PlayerInventory(PlayerTestDataBuilder.BuildPlayer(1000), new Dictionary<Slot, Tuple<IPickupable, ushort>>());

            var result = sut.TryAddItemToSlot(slot, item);

            Assert.Null(result.Value);

            sut.TryAddItemToSlot(slot, newItem);

            Assert.Equal(sut[slot], item);
            Assert.Equal((sut[slot] as Cumulative).Amount, resultItem.Amount);
        }
        [Fact]
        public void AddItemToSlot_When_Item_Is_Cumulative_Raises_Event_When_Reduced()
        {
            var sut = new PlayerInventory(PlayerTestDataBuilder.BuildPlayer(1000), new Dictionary<Slot, Tuple<IPickupable, ushort>>());
            var item = ItemTestData.CreateAmmoItem(100, 100) as AmmoItem;
            var result = sut.TryAddItemToSlot(Slot.Ammo, item);

            var eventRaised = false;
            var itemRemovedEventRaised = false;

            item.OnReduced += (a, b) => eventRaised = true;
            sut.OnItemRemovedFromSlot += (a, b, c, d) =>
            itemRemovedEventRaised = true;

            item.Throw();

            Assert.True(eventRaised);
            Assert.True(itemRemovedEventRaised);
        }
        [Fact]
        public void AddItemToSlot_When_Swap_Item_Is_Cumulative_Raises_Event_When_Reduced()
        {
            var sut = new PlayerInventory(PlayerTestDataBuilder.BuildPlayer(1000), new Dictionary<Slot, Tuple<IPickupable, ushort>>());
            var initialItem = ItemTestData.CreateAmmoItem(101, 1) as AmmoItem;
            var item = ItemTestData.CreateAmmoItem(100, 100) as AmmoItem;

            sut.TryAddItemToSlot(Slot.Ammo, initialItem);
            var result = sut.TryAddItemToSlot(Slot.Ammo, item);

            var eventRaised = false;
            var itemRemovedEventRaised = false;

            item.OnReduced += (a, b) => eventRaised = true;
            sut.OnItemRemovedFromSlot += (a, b, c, d) =>
            itemRemovedEventRaised = true;
            item.Throw();

            Assert.True(eventRaised);
            Assert.True(itemRemovedEventRaised);
        }
        [Fact]
        public void TryAddItemToSlot_SwappedItem_Should_Not_Raise_Event_When_Reduced()
        {
            var sut = new PlayerInventory(PlayerTestDataBuilder.BuildPlayer(1000), new Dictionary<Slot, Tuple<IPickupable, ushort>>());
            var initialItem = ItemTestData.CreateAmmoItem(101, 3) as AmmoItem;
            var item = ItemTestData.CreateAmmoItem(100, 100) as AmmoItem;

            sut.TryAddItemToSlot(Slot.Ammo, initialItem);
            var result = sut.TryAddItemToSlot(Slot.Ammo, item);

            var itemRemovedEventRaised = false;

            var swapped = (result.Value as AmmoItem);

            sut.OnItemRemovedFromSlot += (a, b, c, d) => itemRemovedEventRaised = true;

            swapped.Throw();

            Assert.False(itemRemovedEventRaised);
        }

        [Fact]
        public void AddItemToSlot_AddItem_When_Slot_Has_Cumulative_Item_And_Exceeds_Join_Item_And_Returns_Exceeding_Amount()
        {
            var sut = new PlayerInventory(PlayerTestDataBuilder.BuildPlayer(1000), new Dictionary<Slot, Tuple<IPickupable, ushort>>());

            var result = sut.TryAddItemToSlot(Slot.Ammo, ItemTestData.CreateAmmoItem(100, 50));

            Assert.Null(result.Value);

            result = sut.TryAddItemToSlot(Slot.Ammo, ItemTestData.CreateAmmoItem(100, 80));

            Assert.Equal(30, (result.Value as ICumulative).Amount);
        }

        [Theory]
        [MemberData(nameof(BackpackSlotAddItemsData))]
        public void AddItemToSlot_When_BackpackSlot_Has_Backpack_Add_Item_To_It(IPickupable item)
        {
            var sut = new PlayerInventory(PlayerTestDataBuilder.BuildPlayer(1000), new Dictionary<Slot, Tuple<IPickupable, ushort>>());
            var backpack = ItemTestData.CreateBackpack();

            sut.TryAddItemToSlot(Slot.Backpack, backpack);

            var result = sut.TryAddItemToSlot(Slot.Backpack, item);

            Assert.Null(result.Value);

            Assert.Equal(sut[Slot.Backpack], backpack);
            Assert.Equal(1, backpack.SlotsUsed);
        }
        [Theory]
        [MemberData(nameof(SlotItemsData))]
        public void AddItemToSlot_Changes_Item_Location(Slot slot, IPickupable item)
        {
            var sut = new PlayerInventory(PlayerTestDataBuilder.BuildPlayer(1000), new Dictionary<Slot, Tuple<IPickupable, ushort>>());

            var result = sut.TryAddItemToSlot(slot, item);

            Assert.Equal(Location.Inventory(slot), item.Location);
        }
        [Fact]
        public void SendTo_When_Send_From_Tile_To_Inventory_Must_Remove_From_Tile_And_Add_To_Inventory()
        {
            var sut = new PlayerInventory(PlayerTestDataBuilder.BuildPlayer(1000), new Dictionary<Slot, Tuple<IPickupable, ushort>>());
            var item = ItemTestData.CreateWeaponItem(100, "sword");
            ITile tile = new Tile(new Coordinate(100, 100, 7), TileFlag.None, null, new IItem[0], new IItem[] { item });

            var result = tile.SendTo(sut, tile.TopItemOnStack, 1, 0, (byte)Slot.Left);

            Assert.True(result.IsSuccess);
            Assert.Equal(sut[Slot.Left], item);
            Assert.Null(tile.TopItemOnStack);
        }
        [Fact]
        public void SendTo_When_Send_From_Tile_To_Inventory_Must_Swap_Item()
        {
            var dictionary = new Dictionary<Slot, Tuple<IPickupable, ushort>>();

            var itemOnInventory = ItemTestData.CreateWeaponItem(200, "axe");
            dictionary.Add(Slot.Left, new Tuple<IPickupable, ushort>(itemOnInventory, (ushort)200));

            var sut = new PlayerInventory(PlayerTestDataBuilder.BuildPlayer(1000), dictionary);
            var item = ItemTestData.CreateWeaponItem(100, "sword");
            ITile tile = new Tile(new Coordinate(100, 100, 7), TileFlag.None, null, new IItem[0], new IItem[] { item });

            var result = tile.SendTo(sut, tile.TopItemOnStack, 1, 0, (byte)Slot.Left);

            Assert.True(result.IsSuccess);
            Assert.Equal(sut[Slot.Left], item);
            Assert.Equal(itemOnInventory, tile.TopItemOnStack);
        }
        [Fact]
        public void SendTo_When_Send_Cumulative_From_Tile_To_Inventory_Must_Remove_From_Tile_And_Add_To_Inventory()
        {
            var sut = new PlayerInventory(PlayerTestDataBuilder.BuildPlayer(1000), new Dictionary<Slot, Tuple<IPickupable, ushort>>());

            var item = ItemTestData.CreateAmmoItem(100, 20);
            ITile tile = new Tile(new Coordinate(100, 100, 7), TileFlag.None, null, new IItem[0], new IItem[] { item });

            var result = tile.SendTo(sut, tile.TopItemOnStack, amount: 20, 0, (byte)Slot.Ammo);

            Assert.True(result.IsSuccess);
            Assert.Equal(20, sut[Slot.Ammo].Amount);

            Assert.Null(tile.TopItemOnStack);
        }
        [Fact]
        public void SendTo_When_Send_Cumulative_From_Tile_To_Inventory_Must_Remove_From_Tile_And_Join_To_Inventory()
        {
            var itemOnInventory = ItemTestData.CreateAmmoItem(100, 51);
            var sut = new PlayerInventory(PlayerTestDataBuilder.BuildPlayer(1000), new Dictionary<Slot, Tuple<IPickupable, ushort>>()
            {
                { Slot.Ammo, new Tuple<IPickupable, ushort>(itemOnInventory, 100) }
            });

            var item = ItemTestData.CreateAmmoItem(100, 100);
            ITile tile = new Tile(new Coordinate(100, 100, 7), TileFlag.None, null, new IItem[0], new IItem[] { item });

            var result = tile.SendTo(sut, tile.TopItemOnStack, amount: 100, 0, (byte)Slot.Ammo);

            Assert.False(result.IsSuccess);
            Assert.Equal(InvalidOperation.NotEnoughRoom, result.Error);
            Assert.Equal(100, sut[Slot.Ammo].Amount);

            Assert.Equal(51, tile.TopItemOnStack.Amount);
        }
        [Fact]
        public void SendTo_When_Send_Cumulative_From_Tile_To_Inventory_Do_Nothing_When_Error()
        {
            var itemOnInventory = ItemTestData.CreateWeaponItem(100, "sword");
            var sut = new PlayerInventory(PlayerTestDataBuilder.BuildPlayer(1000), new Dictionary<Slot, Tuple<IPickupable, ushort>>()
            {
                { Slot.Left, new Tuple<IPickupable, ushort>(itemOnInventory, 100) }
            });

            var item = ItemTestData.CreateThrowableDistanceItem(200, 100);
            ITile tile = new Tile(new Coordinate(100, 100, 7), TileFlag.None, null, new IItem[0], new IItem[] { item });

            var result = tile.SendTo(sut, tile.TopItemOnStack, amount: 100, 0, (byte)Slot.Ammo);

            Assert.False(result.IsSuccess);
            Assert.Equal(itemOnInventory, sut[Slot.Left]);

            Assert.Equal(item, tile.TopItemOnStack);
            Assert.Equal(100, tile.TopItemOnStack.Amount);
        }
        [Fact]
        public void SendTo_When_Send_Cumulative_To_Backpack_Move_Item()
        {
            var backpack = ItemTestData.CreateBackpack();
            var sut = new PlayerInventory(PlayerTestDataBuilder.BuildPlayer(1000), new Dictionary<Slot, Tuple<IPickupable, ushort>>()
            {
                { Slot.Backpack, new Tuple<IPickupable, ushort>(backpack, 100) }
            });

            var item = ItemTestData.CreateAmmoItem(200, 100);
            ITile tile = new Tile(new Coordinate(100, 100, 7), TileFlag.None, null, new IItem[0], new IItem[] { item });

            var result = tile.SendTo(sut, tile.TopItemOnStack, amount: 100, 0, (byte)Slot.Backpack);

            Assert.True(result.IsSuccess);
            Assert.Equal(item, (sut[Slot.Backpack] as IContainer)[item.Location.ContainerSlot]);

            Assert.Null(tile.TopItemOnStack);
            Assert.Equal(100, (sut[Slot.Backpack] as IContainer)[item.Location.ContainerSlot].Amount);
        }
        [Fact]
        public void SendTo_When_Send_From_Right_To_Tile_Must_Remove_From_Inventory_And_Add_To_Tile()
        {
            var item = ItemTestData.CreateBodyEquipmentItem(100, "", "shield");

            IInventory sut = new PlayerInventory(PlayerTestDataBuilder.BuildPlayer(1000), new Dictionary<Slot, Tuple<IPickupable, ushort>>()
                 {
                { Slot.Right, new Tuple<IPickupable, ushort>(item,100) }
            });
            ITile tile = new Tile(new Coordinate(100, 100, 7), TileFlag.None, null, new IItem[0], new IItem[] { item });

            var result = sut.SendTo(tile, item, 1, (byte)Slot.Right, 0);

            Assert.True(result.IsSuccess);
            Assert.Null(sut[Slot.Right]);
            Assert.Equal(item, tile.TopItemOnStack);
        }
        [Fact]
        public void SendTo_When_Send_From_Container_To_Ammo_Must_Remove_From_Container_And_Add_To_Inventory()
        {
            var container = ItemTestData.CreateContainer(2);
            var ammo = ItemTestData.CreateAmmoItem(100, 1);
            container.AddThing(ammo);

            IInventory sut = new PlayerInventory(PlayerTestDataBuilder.BuildPlayer(1000), new Dictionary<Slot, Tuple<IPickupable, ushort>>());

            var result = container.SendTo(sut, ammo, 1, (byte)ammo.Location.ContainerSlot, (byte)Slot.Ammo);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, container.FreeSlotsCount);
            Assert.Equal(ammo, sut[Slot.Ammo]);
            Assert.Equal(1, sut[Slot.Ammo].Amount);
        }
        [Fact]
        public void SendTo_When_Send_From_Backpack_To_Ammo_Must_Remove_From_Backpack_And_Add_To_Inventory()
        {
            var backpack = ItemTestData.CreateBackpack();
            var ammo = ItemTestData.CreateAmmoItem(100, 1);
            backpack.AddThing(ammo);

            IInventory sut = new PlayerInventory(PlayerTestDataBuilder.BuildPlayer(1000), new Dictionary<Slot, Tuple<IPickupable, ushort>>()
            {
                {Slot.Backpack, new Tuple<IPickupable, ushort>(backpack, 100) }
            });

            var result = backpack.SendTo(sut, ammo, 1, (byte)ammo.Location.ContainerSlot, (byte)Slot.Ammo);

            Assert.True(result.IsSuccess);
            Assert.Equal(20, backpack.FreeSlotsCount);
            Assert.Equal(ammo, sut[Slot.Ammo]);
            Assert.Equal(1, sut[Slot.Ammo].Amount);
        }
    }
}

