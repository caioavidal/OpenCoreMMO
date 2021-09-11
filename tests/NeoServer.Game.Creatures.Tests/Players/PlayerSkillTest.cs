using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;
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
                {SkillType.Axe, new Skill(SkillType.Axe,1,12,0)}
            });
            var level = player.GetSkillLevel(SkillType.Club);

            Assert.Equal(1, level);
        }
        [Fact]
        public void GetSkillLevel_When_Has_Skill_Returns_Level()
        {
            var player = PlayerTestDataBuilder.BuildPlayer(hp: 100, skills: new Dictionary<SkillType, ISkill>
            {
                {SkillType.Axe, new Skill(SkillType.Axe,1,12,0)}
            });
            var level = player.GetSkillLevel(SkillType.Axe);

            Assert.Equal(12, level);
        }
        [Fact]
        public void GetSkillLevel_When_Has_No_Bonus_Skill_Returns_Level()
        {
            var player = PlayerTestDataBuilder.BuildPlayer(hp: 100, skills: new Dictionary<SkillType, ISkill>
            {
                {SkillType.Axe, new Skill(SkillType.Axe,1,12,0)}
            }, inventory: new Dictionary<Slot, Tuple<IPickupable, ushort>>
            {
                {Slot.Necklace, new Tuple<IPickupable, ushort>(ItemTestData.CreateDefenseEquipmentItem(id: 100),1)}
            });
            var level = player.GetSkillLevel(SkillType.Axe);

            Assert.Equal(12, level);
        }
        [Fact]
        public void GetSkillLevel_When_Has_Bonus_Skill_Returns_Level_Plus_Bonus()
        {
            
            var player = PlayerTestDataBuilder.BuildPlayer(hp: 100, skills: new Dictionary<SkillType, ISkill>
            {
                {SkillType.Axe, new Skill(SkillType.Axe,1,12,0)}
            }, inventory: new Dictionary<Slot, Tuple<IPickupable, ushort>>
            {
                {Slot.Necklace, new Tuple<IPickupable, ushort>(ItemTestData.CreateDefenseEquipmentItem(id: 100),1)}
            });
            var level = player.GetSkillLevel(SkillType.Axe);

            Assert.Equal(12, level);
        }
    }
}
