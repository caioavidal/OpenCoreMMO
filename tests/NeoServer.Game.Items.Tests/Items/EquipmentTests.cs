using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Items.Items;
using NeoServer.Game.Items.Items.Attributes;
using NeoServer.Game.Tests.Helpers;
using Xunit;

namespace NeoServer.Game.Items.Tests.Items
{
    public class EquipmentTests
    {
        [Fact]
        public void DressedIn_Null_DoNotThrow()
        {
            var sut = ItemTestData.CreateDefenseEquipmentItem(1);
            sut.Metadata.Attributes.SetAttribute(ItemAttribute.SkillAxe, 5);
            sut.DressedIn(null);
        }
        [Fact]
        public void DressedIn_Player_AddSkillBonus()
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer(skills: PlayerTestDataBuilder.GenerateSkills(10));
            var sut = ItemTestData.CreateDefenseEquipmentItem(1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.SkillAxe, 5),
                (ItemAttribute.Duration, 100)
            });
            //act
            sut.DressedIn(player);

            //assert
            player.GetSkillBonus(SkillType.Axe).Should().Be(5);
        }

        [Fact]
        public void UndressFrom_Null_DoNotThrow()
        {
            //arrange
            var sut = ItemTestData.CreateDefenseEquipmentItem(1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.SkillAxe, 5),
                (ItemAttribute.Duration, 100)
            });
            //act
            sut.UndressFrom(null);
        }
        [Fact]
        public void UndressFrom_Player_RemoveSkillBonus()
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer(skills: PlayerTestDataBuilder.GenerateSkills(10));
            var sut = ItemTestData.CreateDefenseEquipmentItem(1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.SkillAxe, 5),
                (ItemAttribute.Duration, 100)
            });

            //act
            sut.DressedIn(player);
            //assert
            player.GetSkillBonus(SkillType.Axe).Should().Be(5);

            //act
            sut.UndressFrom(player);
            //assert
            player.GetSkillBonus(SkillType.Axe).Should().Be(0);
        }

        [Fact]
        public void NoCharges_10Charges_ReturnsFalse()
        {
            //arrange
            var sut = ItemTestData.CreateDefenseEquipmentItem(1, charges: 10);
            sut.Metadata.Attributes.SetAttribute(ItemAttribute.AbsorbPercentEnergy, 10);

            //assert
            sut.NoCharges.Should().BeFalse();
        }
        [Fact]
        public void NoCharges_0Charges_ReturnsTrue()
        {
            //arrange
            var sut = ItemTestData.CreateDefenseEquipmentItem(1, charges: 1);
            sut.Metadata.Attributes.SetAttribute(ItemAttribute.AbsorbPercentEnergy, 10);

            //act
            sut.DecreaseCharges();

            //assert
            sut.NoCharges.Should().BeTrue();
        }

        [Fact]
        public void NoCharges_NonChargeable_ReturnsFalse()
        {
            //arrange
            var sut = ItemTestData.CreateDefenseEquipmentItem(1, charges: 0);
            sut.Metadata.Attributes.SetAttribute(ItemAttribute.AbsorbPercentEnergy, 10);

            //assert
            sut.NoCharges.Should().BeFalse();
        }
        [Fact]
        public void TransformOnEquip_NoItemToTransformTo_DoNotTransform()
        {
            //arrange

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, charges: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.AbsorbPercentEnergy, 100),
                (ItemAttribute.Duration, 100)
            });

            var metadata = sut.Metadata;

            //act
            sut.TransformOnEquip();

            //assert
            metadata.Should().BeEquivalentTo(sut.Metadata);

        }

        [Fact]
        public void TransformOnEquip_HasItemToTransformTo_Transform()
        {
            //arrange

            var transformToItem = ItemTestData.CreateDefenseEquipmentItem(2);

            var itemTypeStore = ItemTestData.GetItemTypeStore(transformToItem.Metadata);


            var sut = ItemTestData.CreateDefenseEquipmentItem(1, charges: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.AbsorbPercentEnergy, 100),
                (ItemAttribute.Duration, 100),
                (ItemAttribute.TransformEquipTo,2),

            }, itemTypeFinder: itemTypeStore.Get);

            var metadata = sut.Metadata;

            //act
            sut.TransformOnEquip();

            //assert
            metadata.Should().NotBeEquivalentTo(sut.Metadata);
            sut.Metadata.ClientId.Should().Be(2);
        }

        [Fact]
        public void TransformOnDequip_NoItemToTransformTo_DoNotTransform()
        {
            var itemTypeStore = ItemTestData.GetItemTypeStore();

            //arrange
            var sut = ItemTestData.CreateDefenseEquipmentItem(1, charges: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.AbsorbPercentEnergy, 100),
                (ItemAttribute.Duration, 100)
            }, itemTypeFinder: itemTypeStore.Get);

            var metadata = sut.Metadata;

            //act
            sut.TransformOnDequip();

            //assert
            metadata.Should().BeEquivalentTo(sut.Metadata);
        }
        [Fact]
        public void TransformOnDequip_HasItemToTransformTo_Transform()
        {
            //arrange
            var transformToItem = ItemTestData.CreateDefenseEquipmentItem(2);

            var itemTypeStore = ItemTestData.GetItemTypeStore(transformToItem.Metadata);

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, charges: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.AbsorbPercentEnergy, 100),
                (ItemAttribute.Duration, 100),
                (ItemAttribute.TransformDequipTo,2),

            }, itemTypeFinder: itemTypeStore.Get);

            var metadata = sut.Metadata;

            //act
            sut.TransformOnDequip();

            //assert
            metadata.Should().NotBeEquivalentTo(sut.Metadata);
            sut.Metadata.ClientId.Should().Be(2);
        }

        [Fact]
        public void DressedIn_HasItemToTransformTo_Transform()
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer();
            var transformToItem = ItemTestData.CreateDefenseEquipmentItem(2);

            var itemTypeStore = ItemTestData.GetItemTypeStore(transformToItem.Metadata);

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, charges: 1,
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.AbsorbPercentEnergy, 100),
                    (ItemAttribute.Duration, 100),
                    (ItemAttribute.TransformEquipTo, 2)
                }, itemTypeFinder: itemTypeStore.Get);

            IItemType itemBefore = null;
            IItemType itemNow = null;
            sut.OnTransformed += (before, now) =>
            {
                itemBefore = before;
                itemNow = now;
            };

            var metadata = sut.Metadata;

            //act
            sut.DressedIn(player);

            //assert
            metadata.Should().NotBeEquivalentTo(sut.Metadata);
            sut.Metadata.ClientId.Should().Be(2);
            itemBefore.Should().BeEquivalentTo(metadata);
            itemNow.Should().BeEquivalentTo(sut.Metadata);
        }
        [Fact]
        public void UndressFrom_HasItemToTransformTo_Transform()
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer();

            var transformToItem = ItemTestData.CreateDefenseEquipmentItem(2, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.TransformDequipTo,3)
            });

            var transformOnDequipItem = ItemTestData.CreateDefenseEquipmentItem(3);

            var itemTypeStore = ItemTestData.GetItemTypeStore(transformToItem.Metadata, transformOnDequipItem.Metadata);

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, charges: 1, attributes: new (ItemAttribute, IConvertible)[]
               {
                (ItemAttribute.AbsorbPercentEnergy, 100),
                (ItemAttribute.Duration, 100),
                (ItemAttribute.TransformEquipTo,2)
               }, itemTypeFinder: itemTypeStore.Get);

            IItemType itemBefore = null;
            IItemType itemNow = null;
            sut.OnTransformed += (before, now) =>
            {
                itemBefore = before;
                itemNow = now;
            };

            var metadata = sut.Metadata;


            //assert
            sut.Metadata.ClientId.Should().Be(1);

            //act
            sut.DressedIn(player);

            //assert
            sut.Metadata.ClientId.Should().Be(2);

            var beforeUndress = sut.Metadata;

            sut.UndressFrom(player);

            //assert
            metadata.Should().NotBeEquivalentTo(sut.Metadata);
            sut.Metadata.ClientId.Should().Be(3);
            itemBefore.Should().BeEquivalentTo(beforeUndress);
            itemNow.Should().BeEquivalentTo(sut.Metadata);
        }

        [Fact]
        public void OnDecayed_NoItemToDecayTo_UndressFromPlayer()
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer();

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, slot: "ring", charges: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.AbsorbPercentEnergy, 100),
                (ItemAttribute.Duration, 1),
            });

            var slotRemoved = Slot.None;
            IPickupable itemRemoved = null;
            player.Inventory.OnItemRemovedFromSlot += (_, item, slot, _) =>
            {
                slotRemoved = slot;
                itemRemoved = item;
            };

            //act
            player.Inventory.AddItem(sut, (byte)Slot.Ring);

            Thread.Sleep(1500);
            sut.TryDecay();
            //assert

            player.Inventory[Slot.Ring].Should().BeNull();
            slotRemoved.Should().Be(Slot.Ring);
            itemRemoved.Should().Be(sut);
        }

        [Fact]
        public void CustomLookText_HasCharges_ShowChargesCount()
        {
            //arrange
            var sut = ItemTestData.CreateDefenseEquipmentItem(1, slot: "ring", charges: 2, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.ShowCharges, true)
            });

            //assert
            sut.CustomLookText.Should().Be(" item that has 2 charges left");
            sut.DecreaseCharges();
            sut.CustomLookText.Should().Be(" item that has 1 charge left");
            sut.DecreaseCharges();
            sut.CustomLookText.Should().Be(" item that has no charges left");
        }

        [Fact]
        public void TransformOnEquip_HasDuration_SetsDuration()
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer();

            var transformToItem = ItemTestData.CreateDefenseEquipmentItem(2, slot: "ring", charges: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.AbsorbPercentEnergy, 100),
                (ItemAttribute.ShowDuration, 1),
                (ItemAttribute.Duration, 1800)
            });

            var itemTypeStore = ItemTestData.GetItemTypeStore(transformToItem.Metadata);

            var sut = ItemTestData.CreateDefenseEquipmentItem(id: 1, slot: "ring", charges: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.AbsorbPercentEnergy, 100),
                (ItemAttribute.ShowDuration, 1),
                (ItemAttribute.TransformEquipTo,2)

            }, itemTypeFinder: itemTypeStore.Get);

            //assert
            sut.Duration.Should().Be(0);

            //act
            player.Inventory.AddItem(sut, (byte)Slot.Ring);

            //assert
            sut.Duration.Should().Be(1800);
        }

        [Fact]
        public void StartDecay_NoStopDecayingAttr_Starts()
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer();

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, slot: "ring", charges: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.Duration, 1000),
                (ItemAttribute.ShowDuration, false)
            });

            //assert
            sut.Elapsed.Should().Be(0);

            //act
            player.Inventory.AddItem(sut, (byte)Slot.Ring);
            Thread.Sleep(1500);

            //assert
            sut.Elapsed.Should().BeGreaterThan(0);
        }

        [Fact]
        public void StartDecay_HasStopDecayingTrue_DoNotStart()
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer();

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, slot: "ring", charges: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.Duration, 1000),
                (ItemAttribute.ShowDuration, false),
                (ItemAttribute.StopDecaying, 1),
            });

            //assert
            sut.Elapsed.Should().Be(0);

            //act
            player.Inventory.AddItem(sut, (byte)Slot.Ring);
            Thread.Sleep(1500);

            //assert
            sut.Elapsed.Should().Be(0);
        }
        [Fact]
        public void StartDecay_HasStopDecayingFalse_Start()
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer();

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, slot: "ring", charges: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.Duration, 1000),
                (ItemAttribute.ShowDuration, false),
                (ItemAttribute.StopDecaying, 0),
            });

            //assert
            sut.Elapsed.Should().Be(0);

            //act
            player.Inventory.AddItem(sut, (byte)Slot.Ring);
            Thread.Sleep(1500);

            //assert
            sut.Elapsed.Should().BeGreaterThan(0);
        }
        [Fact]
        public void PauseDecay_HasNoStopDecayingAttr_DoNotPause()
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer();

            var transformToItem = ItemTestData.CreateDefenseEquipmentItem(2, slot: "ring", charges: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.Duration, 100),
                (ItemAttribute.ShowDuration, false),
            });

            var itemTypeStore = ItemTestData.GetItemTypeStore(transformToItem.Metadata);

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, slot: "ring", charges: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.Duration, 100),
                (ItemAttribute.ShowDuration, false),
                (ItemAttribute.TransformEquipTo, 2),
            }, itemTypeFinder: itemTypeStore.Get);

            //assert
            sut.Elapsed.Should().Be(0);

            //act
            player.Inventory.AddItem(sut, (byte)Slot.Ring);
            Thread.Sleep(1100);
            player.Inventory.RemoveItemFromSlot(Slot.Ring, 1, out _);
            Thread.Sleep(1100);

            //assert
            sut.Elapsed.Should().Be(2);
        }
        [Fact]
        public void PauseDecay_HasStopDecayingTrue_Pauses()
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer();

            var dequipTo = ItemTestData.CreateDefenseEquipmentItem(id: 3,
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.StopDecaying, 1)
                });
            var itemTypeStore = ItemTestData.GetItemTypeStore(dequipTo.Metadata);

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, slot: "ring", charges: 1,
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.Duration, 100),
                    (ItemAttribute.ShowDuration, 1),
                    (ItemAttribute.TransformDequipTo, 3),
                }, itemTypeFinder: itemTypeStore.Get);

            //assert
            sut.Elapsed.Should().Be(0);

            //act
            player.Inventory.AddItem(sut, (byte)Slot.Ring);
            Thread.Sleep(1000);
            player.Inventory.RemoveItemFromSlot(Slot.Ring, 1, out _);
            Thread.Sleep(2000);

            //assert
            sut.Elapsed.Should().Be(1);
        }
        [Fact]
        public void PauseDecay_HasStopDecayingFalse_DoNotPause()
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer();
            var transformToItem = ItemTestData.CreateDefenseEquipmentItem(2, slot: "ring", charges: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.Duration, 100),
                (ItemAttribute.ShowDuration, false),
                (ItemAttribute.StopDecaying, 0),

            });

            var transformToItemDequip = ItemTestData.CreateDefenseEquipmentItem(3, slot: "ring", charges: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.Duration, 100),
                (ItemAttribute.ShowDuration, 1),
                (ItemAttribute.StopDecaying, 0)

            });

            var itemTypeStore = ItemTestData.GetItemTypeStore(transformToItemDequip.Metadata, transformToItem.Metadata);

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, slot: "ring", charges: 1,
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.Duration, 100),
                    (ItemAttribute.ShowDuration, false),
                    (ItemAttribute.TransformEquipTo, 2),
                    (ItemAttribute.TransformDequipTo, 3),
                }, itemTypeFinder: itemTypeStore.Get);

            //assert
            sut.Elapsed.Should().Be(0);

            //act
            player.Inventory.AddItem(sut, (byte)Slot.Ring);
            Thread.Sleep(1100);
            player.Inventory.RemoveItemFromSlot(Slot.Ring, 1, out _);
            Thread.Sleep(2000);

            //assert
            sut.Elapsed.Should().Be(3);
        }
    }
}
