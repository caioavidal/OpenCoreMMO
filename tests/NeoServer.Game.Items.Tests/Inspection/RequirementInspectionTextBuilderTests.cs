using System;
using System.Linq;
using System.Text;
using FluentAssertions;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Creatures.Vocations;
using NeoServer.Game.DataStore;
using NeoServer.Game.Items.Inspection;
using NeoServer.Game.Tests.Helpers;
using Xunit;

namespace NeoServer.Game.Items.Tests.Inspection
{
    public class RequirementInspectionTextBuilderTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("It can only be wielded properly by knights and paladins.\r\n", 1, 2)]
        [InlineData("It can only be wielded properly by knights, paladins and sorcerers.\r\n", 1, 2, 3)]
        [InlineData("It can only be wielded properly by knights, paladins, sorcerers and druids.\r\n", 1, 2, 3, 4)]
        [InlineData("It can only be wielded properly by knights, sorcerers and druids.\r\n", 1, 10, 3, 4)]
        public void Add_HasVocations_ReturnText(string expected, params int[] vocations)
        {
            VocationStore.Data.Add(1, new Vocation() { Name = "Knight" });
            VocationStore.Data.Add(2, new Vocation() { Name = "Paladin" });
            VocationStore.Data.Add(3, new Vocation() { Name = "Sorcerer" });
            VocationStore.Data.Add(4, new Vocation() { Name = "Druid" });

            var input = vocations.Select(x => (byte)x).ToArray();

            var sut = ItemTestData.CreateDefenseEquipmentItem(1);
            sut.Metadata.Attributes.SetAttribute(ItemAttribute.Vocation, input);

            var stringBuilder = new StringBuilder();

            RequirementInspectionTextBuilder.Add(sut, stringBuilder);
            stringBuilder.ToString().Should().Be(expected);
        }

        [Theory]
        [InlineData("It can only be wielded properly by players of level 10 or higher.\r\n", 10)]
        [InlineData("It can only be wielded properly by players of level 1 or higher.\r\n", 1)]
        [InlineData("It can only be wielded properly by players of level 200 or higher.\r\n", 200)]
        [InlineData("", 0)]
        public void Add_HasLevel_ReturnText(string expected, int level)
        {
            var sut = ItemTestData.CreateDefenseEquipmentItem(1);
            sut.Metadata.Attributes.SetAttribute(ItemAttribute.MinimumLevel, level);

            var stringBuilder = new StringBuilder();

            RequirementInspectionTextBuilder.Add(sut, stringBuilder);
            stringBuilder.ToString().Should().Be(expected);
        }

        [Theory]
        [InlineData("It can only be wielded properly by knights of level 10 or higher.\r\n", 10, 1)]
        [InlineData("It can only be wielded properly by knights and paladins of level 1 or higher.\r\n", 1, 1, 2)]
        [InlineData("It can only be wielded properly by knights, paladins and sorcerers of level 200 or higher.\r\n", 200, 1, 2, 3)]
        [InlineData("", 0)]
        public void Add_HasLevelAndVocations_ReturnText(string expected, int level, params int[] vocations)
        {
            VocationStore.Data.Add(1, new Vocation() { Name = "Knight" });
            VocationStore.Data.Add(2, new Vocation() { Name = "Paladin" });
            VocationStore.Data.Add(3, new Vocation() { Name = "Sorcerer" });
            VocationStore.Data.Add(4, new Vocation() { Name = "Druid" });

            var input = vocations.Select(x => (byte)x).ToArray();

            var sut = ItemTestData.CreateDefenseEquipmentItem(1);
            sut.Metadata.Attributes.SetAttribute(ItemAttribute.MinimumLevel, level);
            sut.Metadata.Attributes.SetAttribute(ItemAttribute.Vocation, input);

            var stringBuilder = new StringBuilder();

            RequirementInspectionTextBuilder.Add(sut, stringBuilder);
            stringBuilder.ToString().Should().Be(expected);
        }
        
        [Fact]
        public void Add_HasNoRequirement_ReturnEmpty()
        {
            var sut = ItemTestData.CreateCoin(1,10,1);
         
            var stringBuilder = new StringBuilder();

            RequirementInspectionTextBuilder.Add(sut, stringBuilder);
            stringBuilder.ToString().Should().BeEmpty();
        }
    }
}