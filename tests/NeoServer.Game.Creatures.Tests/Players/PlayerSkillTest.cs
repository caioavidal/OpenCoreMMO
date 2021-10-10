using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Creatures.Model.Players;
using NeoServer.Game.Tests.Helpers;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.Players
{
    public class PlayerSkillTest
    {
        [Fact]
        public void GetSkillLevel_When_Has_No_Skill_Returns_1()
        {
            var player = PlayerTestDataBuilder.BuildPlayer(hp: 100, skills: new Dictionary<SkillType, ISkill>
            {
                { SkillType.Axe, new Skill(SkillType.Axe, 1, 12, 0) }
            });
            var level = player.GetSkillLevel(SkillType.Club);

            Assert.Equal(1, level);
        }

        [Fact]
        public void GetSkillLevel_When_Has_Skill_Returns_Level()
        {
            var player = PlayerTestDataBuilder.BuildPlayer(hp: 100, skills: new Dictionary<SkillType, ISkill>
            {
                { SkillType.Axe, new Skill(SkillType.Axe, 1, 12, 0) }
            });
            var level = player.GetSkillLevel(SkillType.Axe);

            Assert.Equal(12, level);
        }

        [Fact]
        public void Player_wearing_a_non_skill_bonus_item_skill_remains_the_same()
        {
            var player = PlayerTestDataBuilder.BuildPlayer(hp: 100, skills: new Dictionary<SkillType, ISkill>
            {
                { SkillType.Axe, new Skill(SkillType.Axe, 1, 12, 0) }
            }, inventory: new Dictionary<Slot, Tuple<IPickupable, ushort>>
            {
                { Slot.Necklace, new Tuple<IPickupable, ushort>(ItemTestData.CreateDefenseEquipmentItem(id: 100,slot:"necklace"), 1) }
            });
            var level = player.GetSkillLevel(SkillType.Axe);

            Assert.Equal(12, level);
        }

        [Fact]
        public void Player_wearing_a_skill_bonus_item_has_skill_increased()
        {
            //arrange
            var necklace = ItemTestData.CreateDefenseEquipmentItem(id: 100, slot:"necklace",
                attributes: new (ItemAttribute, IConvertible)[]
                {
                    (ItemAttribute.SkillAxe, 5),
                });

            var player = PlayerTestDataBuilder.BuildPlayer(hp: 100, skills: new Dictionary<SkillType, ISkill>
            {
                { SkillType.Axe, new Skill(SkillType.Axe, 1, 12, 0) }
            }, inventory: new Dictionary<Slot, Tuple<IPickupable, ushort>>
            {
                { Slot.Necklace, new Tuple<IPickupable, ushort>(necklace, 1) }
            });

            //act
            var result = player.GetSkillLevel(SkillType.Axe);

            //assert
            result.Should().Be(17);
        }
    }
}