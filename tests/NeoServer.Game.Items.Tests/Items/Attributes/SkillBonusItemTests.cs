using System;
using System.Collections.Generic;
using FluentAssertions;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Tests.Helpers;
using Xunit;

namespace NeoServer.Game.Items.Tests.Items.Attributes
{
    public class SkillBonusItemTests
    {
        [Fact]
        public void AddSkillBonus_Null_DoNotThrow()
        {
            var sut = ItemTestData.CreateDefenseEquipmentItem(1);

            sut.AddSkillBonus(null);
        }

        [Fact]
        public void AddSkillBonus_ToPlayer_AddSkills()
        {
            var skills = PlayerTestDataBuilder.GenerateSkills(10);
            var player = PlayerTestDataBuilder.BuildPlayer(skills: skills);

            var sut = ItemTestData.CreateDefenseEquipmentItem(id:1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.SkillAxe, 5),
                (ItemAttribute.SkillShield, 15)
            });

            sut.AddSkillBonus(player);
            
            player.GetSkillBonus(SkillType.Axe).Should().Be(5);
            player.GetSkillBonus(SkillType.Sword).Should().Be(0);
            player.GetSkillBonus(SkillType.Distance).Should().Be(0);
            player.GetSkillBonus(SkillType.Fishing).Should().Be(0);
            player.GetSkillBonus(SkillType.Fist).Should().Be(0);
            player.GetSkillBonus(SkillType.Level).Should().Be(0);
            player.GetSkillBonus(SkillType.Magic).Should().Be(0);
            player.GetSkillBonus(SkillType.Shielding).Should().Be(15);
            player.GetSkillBonus(SkillType.Speed).Should().Be(0);
            player.GetSkillBonus(SkillType.Club).Should().Be(0);
        }
        [Fact]
        public void RemoveSkillBonus_Null_DoNotThrow()
        {
            var sut = ItemTestData.CreateDefenseEquipmentItem(1);

            sut.RemoveSkillBonus(null);
        }

        [Fact]
        public void RemoveSkillBonus_ToPlayer_RemoveSkills()
        {
            var skills = PlayerTestDataBuilder.GenerateSkills(10);
            var player = PlayerTestDataBuilder.BuildPlayer(skills: skills);

            var sut = ItemTestData.CreateDefenseEquipmentItem(1);
            sut.Metadata.Attributes.SetAttribute(ItemAttribute.SkillAxe, 5);
            sut.Metadata.Attributes.SetAttribute(ItemAttribute.SkillShield, 15);


            sut.AddSkillBonus(player);

            sut.RemoveSkillBonus(player);
            
            player.GetSkillBonus(SkillType.Axe).Should().Be(0);
            player.GetSkillBonus(SkillType.Sword).Should().Be(0);
            player.GetSkillBonus(SkillType.Distance).Should().Be(0);
            player.GetSkillBonus(SkillType.Fishing).Should().Be(0);
            player.GetSkillBonus(SkillType.Fist).Should().Be(0);
            player.GetSkillBonus(SkillType.Level).Should().Be(0);
            player.GetSkillBonus(SkillType.Magic).Should().Be(0);
            player.GetSkillBonus(SkillType.Shielding).Should().Be(0);
            player.GetSkillBonus(SkillType.Speed).Should().Be(0);
            player.GetSkillBonus(SkillType.Club).Should().Be(0);
        }


        [Fact]
        public void AddSkillBonus_2ItemsToPlayer_AddSkills()
        {
            var skills = PlayerTestDataBuilder.GenerateSkills(10);
            var player = PlayerTestDataBuilder.BuildPlayer(skills: skills);

            var item1 = ItemTestData.CreateDefenseEquipmentItem(id: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.SkillAxe, 5),
                (ItemAttribute.SkillShield, 15)
            });
            var item2 = ItemTestData.CreateDefenseEquipmentItem(id: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.SkillAxe, 15),
                (ItemAttribute.SkillShield, 25)
            });

            item1.AddSkillBonus(player);
            item2.AddSkillBonus(player);

            player.GetSkillBonus(SkillType.Axe).Should().Be(20);
         
            player.GetSkillBonus(SkillType.Shielding).Should().Be(40);
          
        }

        [Fact]
        public void ChangeSkillBonus_Changes()
        {
            //arrange
            var skills = PlayerTestDataBuilder.GenerateSkills(10);
            var player = PlayerTestDataBuilder.BuildPlayer(skills: skills);

            var item1 = ItemTestData.CreateDefenseEquipmentItem(id: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.SkillAxe, 5),
            });

            //act
            item1.AddSkillBonus(player);
            
            //assert
            player.GetSkillBonus(SkillType.Axe).Should().Be(5);

            //act
            item1.RemoveSkillBonus(player);
            item1.ChangeSkillBonuses(new Dictionary<SkillType, byte>()
            {
                [SkillType.Sword] = 10
            });
            item1.AddSkillBonus(player);

            //assert
            player.GetSkillBonus(SkillType.Axe).Should().Be(0);
            player.GetSkillBonus(SkillType.Sword).Should().Be(10);

        }
        [Fact]
        public void ChangeSkillBonus_Null_DoNotChange()
        {
            //arrange
            var skills = PlayerTestDataBuilder.GenerateSkills(10);
            var player = PlayerTestDataBuilder.BuildPlayer(skills: skills);

            var item1 = ItemTestData.CreateDefenseEquipmentItem(id: 1, attributes: new (ItemAttribute, IConvertible)[]
            {
                (ItemAttribute.SkillAxe, 5),
            });

            //act
            item1.AddSkillBonus(player);

            //assert
            player.GetSkillBonus(SkillType.Axe).Should().Be(5);

            //act
            item1.RemoveSkillBonus(player);
            item1.ChangeSkillBonuses(null);
            item1.AddSkillBonus(player);

            //assert
            player.GetSkillBonus(SkillType.Axe).Should().Be(5);
            player.GetSkillBonus(SkillType.Sword).Should().Be(0);

        }

    }
}
