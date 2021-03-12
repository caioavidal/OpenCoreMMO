using Moq;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.DataStore;
using NeoServer.Game.Tests;
using NeoServer.Server.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.WalkableCreature
{
    public partial class PlayerTest
    {
        [Fact]
        public void HasNextStep_Returns_True_When_Player_Has_Steps_To_Walk()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100, skills: new Dictionary<SkillType, ISkill>
              {
                    { SkillType.Level, new Skill(SkillType.Level, 1.1f,100,0)  }

              });


            Assert.False(sut.HasNextStep);
            sut.WalkTo(Direction.South, Direction.North);
            Assert.True(sut.HasNextStep);
        }

        [Fact]
        public void IsFollowing_Returns_True_When_Player_Is_Following_Someone()
        {
            var pathFinder = new Mock<IPathFinder>();
            var directions = new Direction[] { Direction.North };
            pathFinder.Setup(x => x.Find(It.IsAny<ICreature>(), It.IsAny<Location>(), It.IsAny<FindPathParams>(), It.IsAny<ITileEnterRule>(), out directions)).Returns(true);
            ConfigurationStore.PathFinder = pathFinder.Object;

            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100, skills: new Dictionary<SkillType, ISkill>
              {
                    { SkillType.Level, new Skill(SkillType.Level, 1.1f,100,0)  }
              });

            var creature = new Mock<ICreature>();
            creature.Setup(x => x.Location).Returns(sut.Location);
            creature.Setup(x => x.CreatureId).Returns(123);

            Assert.False(sut.IsFollowing);
            sut.Follow(creature.Object);
            Assert.True(sut.IsFollowing);
        }
    }
}
