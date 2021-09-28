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
using NeoServer.Game.DataStore;
using NeoServer.Game.Items.Bases;
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

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, charges: 1,
                attributes: new (ItemAttribute, IConvertible)[]
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


            var sut = ItemTestData.CreateDefenseEquipmentItem(1, charges: 1,
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.AbsorbPercentEnergy, 100),
                    (ItemAttribute.Duration, 100),
                    (ItemAttribute.TransformEquipTo, 2),
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
            var sut = ItemTestData.CreateDefenseEquipmentItem(1, charges: 1,
                attributes: new (ItemAttribute, IConvertible)[]
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

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, charges: 1,
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.AbsorbPercentEnergy, 100),
                    (ItemAttribute.Duration, 100),
                    (ItemAttribute.TransformDequipTo, 2),
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

            var transformToItem = ItemTestData.CreateDefenseEquipmentItem(2,
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.TransformDequipTo, 3)
                });

            var transformOnDequipItem = ItemTestData.CreateDefenseEquipmentItem(3);

            var itemTypeStore = ItemTestData.GetItemTypeStore(transformToItem.Metadata, transformOnDequipItem.Metadata);

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

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, slot: "ring", charges: 1,
                attributes: new (ItemAttribute, IConvertible)[]
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

        #region InspectionText

        [Fact]
        public void InspectionText_HasCharges_ShowChargesCount()
        {
            //arrange
            var sut = ItemTestData.CreateDefenseEquipmentItem(1, slot: "ring", charges: 2,
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.ShowCharges, true)
                });

            //assert
            sut.InspectionText.Should().Be(" that has 2 charges left");
            sut.DecreaseCharges();
            sut.InspectionText.Should().Be(" that has 1 charge left");
            sut.DecreaseCharges();
            sut.InspectionText.Should().Be(" that has no charges left");
        }

        [Fact]
        public void InspectionText_HasAttribute_ReturnText()
        {
            //arrange
            var sut = ItemTestData.CreateWeaponItem(1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.Attack, 50),
                (ItemAttribute.Defense, 20)
            });

            //assert
            sut.InspectionText.Should().Be("(Atk: 50, Def: 20)");
        }

        [Fact]
        public void InspectionText_HasAttributesAndSkillBonus_ReturnText()
        {
            //arrange
            var sut = ItemTestData.CreateWeaponItem(1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.Attack, 50),
                (ItemAttribute.Defense, 20),
                (ItemAttribute.SkillAxe, 30),
                (ItemAttribute.SkillClub, 10)
            });

            //assert
            sut.InspectionText.Should().Be("(Atk: 50, Def: 20, axe fighting +30, club fighting +10)");
        }

        [Fact]
        public void InspectionText_HasAttributesAndSkillBonusAndProtection_ReturnText()
        {
            //arrange
            var sut = ItemTestData.CreateWeaponItem(1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.Attack, 50),
                (ItemAttribute.Defense, 20),
                (ItemAttribute.SkillAxe, 30),
                (ItemAttribute.SkillClub, 10),
                (ItemAttribute.AbsorbPercentDeath, 60),
                (ItemAttribute.AbsorbPercentEnergy, 70)
            });

            //assert
            sut.InspectionText.Should()
                .Be("(Atk: 50, Def: 20, axe fighting +30, club fighting +10, protection death +60%, energy +70%)");
        }

        [Fact]
        public void InspectionText_AllAttributesAndDecay_ReturnText()
        {
            //arrange
            var sut = ItemTestData.CreateWeaponItem(1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.Attack, 50),
                (ItemAttribute.Defense, 20),
                (ItemAttribute.SkillAxe, 30),
                (ItemAttribute.SkillClub, 10),
                (ItemAttribute.AbsorbPercentDeath, 60),
                (ItemAttribute.AbsorbPercentEnergy, 70),
                (ItemAttribute.Duration, 50),
                (ItemAttribute.ShowDuration, 1),
                (ItemAttribute.StopDecaying, 0)
            });

            //assert
            sut.InspectionText.Should()
                .Be(
                    "(Atk: 50, Def: 20, axe fighting +30, club fighting +10, protection death +60%, energy +70%) that is brand-new");

            (sut as IEquipment).StartDecay();
            sut.InspectionText.Should()
                .Be(
                    "(Atk: 50, Def: 20, axe fighting +30, club fighting +10, protection death +60%, energy +70%) that will expire in 0 minute and 49 seconds");
        }

        [Fact]
        public void InspectionText_AllAttributesAndCharges_ReturnText()
        {
            //arrange
            var sut = ItemTestData.CreateWeaponItem(1, charges:10, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.Attack, 50),
                (ItemAttribute.Defense, 20),
                (ItemAttribute.SkillAxe, 30),
                (ItemAttribute.SkillClub, 10),
                (ItemAttribute.AbsorbPercentDeath, 60),
                (ItemAttribute.AbsorbPercentEnergy, 70),
                (ItemAttribute.ShowCharges, 1)
            });

            //assert
            sut.InspectionText.Should()
                .Be(
                    "(Atk: 50, Def: 20, axe fighting +30, club fighting +10, protection death +60%, energy +70%) that has 10 charges left");
        }

        #endregion

        [Fact]
        public void TransformOnEquip_HasDuration_SetsDuration()
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer();

            var transformToItem = ItemTestData.CreateDefenseEquipmentItem(2, slot: "ring", charges: 1,
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.AbsorbPercentEnergy, 100),
                    (ItemAttribute.ShowDuration, 1),
                    (ItemAttribute.Duration, 1800)
                });

            var itemTypeStore = ItemTestData.GetItemTypeStore(transformToItem.Metadata);

            var sut = ItemTestData.CreateDefenseEquipmentItem(id: 1, slot: "ring", charges: 1,
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.AbsorbPercentEnergy, 100),
                    (ItemAttribute.ShowDuration, 1),
                    (ItemAttribute.TransformEquipTo, 2)
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

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, slot: "ring", charges: 1,
                attributes: new (ItemAttribute, IConvertible)[]
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

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, slot: "ring", charges: 1,
                attributes: new (ItemAttribute, IConvertible)[]
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

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, slot: "ring", charges: 1,
                attributes: new (ItemAttribute, IConvertible)[]
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

            var transformToItem = ItemTestData.CreateDefenseEquipmentItem(2, slot: "ring", charges: 1,
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.Duration, 100),
                    (ItemAttribute.ShowDuration, false),
                });

            var itemTypeStore = ItemTestData.GetItemTypeStore(transformToItem.Metadata);

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, slot: "ring", charges: 1,
                attributes: new (ItemAttribute, IConvertible)[]
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
            sut.Elapsed.Should().Be(3);
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
            var transformToItem = ItemTestData.CreateDefenseEquipmentItem(2, slot: "ring", charges: 1,
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.Duration, 100),
                    (ItemAttribute.ShowDuration, false),
                    (ItemAttribute.StopDecaying, 0),
                });

            var transformToItemDequip = ItemTestData.CreateDefenseEquipmentItem(3, slot: "ring", charges: 1,
                attributes: new (ItemAttribute, IConvertible)[]
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
            sut.Elapsed.Should().Be(4);
        }

        [Fact]
        public void Decayed_HasExpirationTarget_ChangeItem()
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer();

            var decaysTo = ItemTestData.CreateDefenseEquipmentItem(id: 3, slot: "ring",
                attributes: new (ItemAttribute, IConvertible)[]
                {
                });
            var itemTypeStore = ItemTestData.GetItemTypeStore(decaysTo.Metadata);

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, slot: "ring", charges: 1,
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.Duration, 1),
                    (ItemAttribute.ShowDuration, 1),
                    (ItemAttribute.ExpireTarget, 3),
                }, itemTypeFinder: itemTypeStore.Get);

            //act
            player.Inventory.AddItem(sut, (byte)Slot.Ring);

            Thread.Sleep(1200);
            sut.TryDecay();

            //assert
            player.Inventory[Slot.Ring].Metadata.Should().Be(decaysTo.Metadata);
        }

        [Fact]
        public void TransformOnEquip_OldItemHasNoDecayableButNewHas_CreateDecayableInstance()
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer();

            var transformOnEquip = ItemTestData.CreateDefenseEquipmentItem(id: 3, slot: "ring",
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.Duration, 100),
                    (ItemAttribute.ShowDuration, 1),
                    (ItemAttribute.ExpireTarget, 0),
                });
            var itemTypeStore = ItemTestData.GetItemTypeStore(transformOnEquip.Metadata);

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, slot: "ring", charges: 1,
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.TransformEquipTo, 3)
                }, itemTypeFinder: itemTypeStore.Get);

            //act
            player.Inventory.AddItem(sut, (byte)Slot.Ring);

            Thread.Sleep(1200);
            //assert
            player.Inventory[Slot.Ring].Metadata.Should().Be(transformOnEquip.Metadata);
            (player.Inventory[Slot.Ring] as IEquipment).Duration.Should().Be(100);
        }

        [Fact]
        public void Decayed_HasExpirationTargetButNoFound_OnlyRemovesItem()
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer();

            var itemTypeStore = ItemTestData.GetItemTypeStore();

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, slot: "ring", charges: 1,
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.Duration, 1),
                    (ItemAttribute.ShowDuration, 1),
                    (ItemAttribute.ExpireTarget, 5),
                }, itemTypeFinder: itemTypeStore.Get);

            //act
            player.Inventory.AddItem(sut, (byte)Slot.Ring);
            Thread.Sleep(1100);
            sut.TryDecay();

            //assert
            player.Inventory[Slot.Ring].Should().BeNull();
        }


        [Fact]
        public void Decayed_Changes3Times_ShouldDecay()
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer();

            ItemTypeStore itemTypeStore = ItemTestData.GetItemTypeStore();

            var item3Equipped = ItemTestData.CreateDefenseEquipmentItem(id: 6, slot: "ring",
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.Duration, 1),
                    (ItemAttribute.ShowDuration, 1),
                    (ItemAttribute.ExpireTarget, 0),
                    (ItemAttribute.TransformDequipTo, 5),
                });

            var item3 = ItemTestData.CreateDefenseEquipmentItem(5, slot: "ring", charges: 1,
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.TransformEquipTo, 6),
                    (ItemAttribute.StopDecaying, 1),
                    (ItemAttribute.ShowDuration, 1)
                }, itemTypeFinder: itemTypeStore.Get);
            var item2Equipped = ItemTestData.CreateDefenseEquipmentItem(id: 4, slot: "ring",
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.Duration, 1),
                    (ItemAttribute.ShowDuration, 1),
                    (ItemAttribute.ExpireTarget, 5),
                    (ItemAttribute.TransformDequipTo, 3),
                });

            var item2 = ItemTestData.CreateDefenseEquipmentItem(3, slot: "ring", charges: 1,
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.TransformEquipTo, 4),
                    (ItemAttribute.StopDecaying, 1),
                    (ItemAttribute.ShowDuration, 1)
                }, itemTypeFinder: itemTypeStore.Get);

            var item1Equipped = ItemTestData.CreateDefenseEquipmentItem(id: 2, slot: "ring",
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.Duration, 2),
                    (ItemAttribute.ShowDuration, 1),
                    (ItemAttribute.ExpireTarget, 3),
                    (ItemAttribute.TransformDequipTo, 1),
                });

            var item1 = ItemTestData.CreateDefenseEquipmentItem(1, slot: "ring", charges: 1,
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.TransformEquipTo, 2),
                    (ItemAttribute.StopDecaying, 1),
                    (ItemAttribute.ShowDuration, 1)
                }, itemTypeFinder: itemTypeStore.Get);

            ItemTestData.AddItemTypeStore(itemTypeStore, item1.Metadata, item1Equipped.Metadata, item2.Metadata,
                item2Equipped.Metadata, item3.Metadata, item3Equipped.Metadata);

            //assert first item
            item1.Duration.Should().Be(0);
            item1.Metadata.Should().Be(item1.Metadata);

            //act
            player.Inventory.AddItem(item1, (byte)Slot.Ring);

            //assert first item equipped
            item1.Duration.Should().Be(2);
            item1.Metadata.Should().Be(item1Equipped.Metadata);

            //act
            Thread.Sleep(2200);
            item1.TryDecay();

            //assert second item equipped
            item1.Duration.Should().Be(1);
            item1.Metadata.Should().Be(item2Equipped.Metadata);

            //act
            Thread.Sleep(1200);
            item1.TryDecay();

            //assert third item equipped
            item1.Duration.Should().Be(1);
            item1.Metadata.Should().Be(item3Equipped.Metadata);

            //act
            Thread.Sleep(1200);
            item1.TryDecay();

            //assert player
            player.Inventory[Slot.Ring].Should().BeNull();
        }


        [Fact]
        public void TransformOnEquip_OldItemHasNoSkillBonusButNewHas_CreateSkillBonusInstance()
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer();

            var transformOnEquip = ItemTestData.CreateDefenseEquipmentItem(id: 3, slot: "ring",
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.SkillAxe, 5)
                });
            var itemTypeStore = ItemTestData.GetItemTypeStore(transformOnEquip.Metadata);

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, slot: "ring", charges: 1,
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.TransformEquipTo, 3)
                }, itemTypeFinder: itemTypeStore.Get);

            //act
            player.Inventory.AddItem(sut, (byte)Slot.Ring);

            //assert
            player.Inventory[Slot.Ring].Metadata.Should().Be(transformOnEquip.Metadata);
            player.GetSkillBonus(SkillType.Axe).Should().Be(5);
        }

        [Fact]
        public void TransformOnEquip_OldItemHasNoProtectionButNewHas_CreateProtectionInstance()
        {
            //arrange
            var player = PlayerTestDataBuilder.BuildPlayer();

            var combatDamage = new CombatDamage(100, DamageType.Death);

            var transformOnEquip = ItemTestData.CreateDefenseEquipmentItem(id: 3, slot: "ring",
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.AbsorbPercentDeath, 5)
                });
            var itemTypeStore = ItemTestData.GetItemTypeStore(transformOnEquip.Metadata);

            var sut = ItemTestData.CreateDefenseEquipmentItem(1, slot: "ring", charges: 1,
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.TransformEquipTo, 3)
                }, itemTypeFinder: itemTypeStore.Get);

            //act
            player.Inventory.AddItem(sut, (byte)Slot.Ring);

            //assert

            player.Inventory[Slot.Ring].Metadata.Should().Be(transformOnEquip.Metadata);
            sut.Protect(ref combatDamage);
            combatDamage.Damage.Should().Be(95);
        }
    }
}