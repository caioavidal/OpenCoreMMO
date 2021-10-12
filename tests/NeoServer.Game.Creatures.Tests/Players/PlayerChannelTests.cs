using System.Collections.Generic;
using FluentAssertions;
using NeoServer.Game.Chats;
using NeoServer.Game.Chats.Rules;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Creatures.Model.Players;
using NeoServer.Game.Tests.Helpers;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.Players
{
    public class PlayerChannelTests
    {
        [Fact]
        public void Player_adds_personal_channel()
        {
            //arrange
            var sut = PlayerTestDataBuilder.Build();
            var chatChannel = new ChatChannel(1, "my channel");
            //act
            sut.Channel.AddPersonalChannel(chatChannel);
            
            //arrange
            sut.Channel.PersonalChannels.Should().Contain(chatChannel);
        }
        [Fact]
        public void Player_cannot_add_an_invalid_personal_channel()
        {
            //arrange
            var sut = PlayerTestDataBuilder.Build();
            //act
            sut.Channel.AddPersonalChannel(null);
            
            //arrange
            sut.Channel.PersonalChannels.Should().BeNullOrEmpty();
        }
        
        [Fact]
        public void Player_cannot_join_invalid_channel()
        {
            //arrange
            var sut = PlayerTestDataBuilder.Build();
            using var monitor = sut.Channel.Monitor();
            //act
            var result = sut.Channel.JoinChannel(null);
            
            //arrange
            result.Should().BeFalse();
            sut.Channel.PersonalChannels.Should().BeNullOrEmpty();
            sut.Channel.PrivateChannels.Should().BeNullOrEmpty();
            
            monitor.Should().NotRaise(nameof(sut.Channel.OnJoinedChannel));
        }
        
        [Fact]
        public void Player_cannot_join_channel_when_already_in()
        {
            //arrange
            var sut = PlayerTestDataBuilder.Build();
            using var sutMonitor = sut.Channel.Monitor();

            var channel = new ChatChannel(1, "channel 1");
            channel.AddUser(sut);
            //act
            var result = sut.Channel.JoinChannel(channel);
            
            //arrange
            result.Should().BeFalse();
            sut.Channel.PersonalChannels.Should().BeNullOrEmpty();
            sut.Channel.PrivateChannels.Should().BeNullOrEmpty();
            
            sutMonitor.Should().NotRaise(nameof(sut.Channel.OnJoinedChannel));
        }
        
        [Fact]
        public void Player_cannot_join_channel_with_unmeeting_rules()
        {
            //arrange
            var sut = PlayerTestDataBuilder.Build(skills: new Dictionary<SkillType, ISkill>()
            {
                [SkillType.Level] = new Skill(SkillType.Level,0,1,0)
            });
            
            using var sutMonitor = sut.Channel.Monitor();

            var channel = new ChatChannel(1, "channel 1")
            {
                JoinRule = new ChannelRule()
                {
                    MinMaxAllowedLevel = (100, 200)
                }
            };
            
            //act
            var result = sut.Channel.JoinChannel(channel);
            
            //arrange
            result.Should().BeFalse();
            sut.Channel.PersonalChannels.Should().BeNullOrEmpty();
            sut.Channel.PrivateChannels.Should().BeNullOrEmpty();
            
            sutMonitor.Should().NotRaise(nameof(sut.Channel.OnJoinedChannel));
        }
        
        [Fact]
        public void Player_joins_channel()
        {
            //arrange
            var sut = PlayerTestDataBuilder.Build(skills: new Dictionary<SkillType, ISkill>()
            {
                [SkillType.Level] = new Skill(SkillType.Level,0,101,0)
            });
            
            using var sutMonitor = sut.Channel.Monitor();

            var channel = new ChatChannel(1, "channel 1")
            {
                JoinRule = new ChannelRule()
                {
                    MinMaxAllowedLevel = (100, 200)
                }
            };
            
            //act
            var result = sut.Channel.JoinChannel(channel);
            
            //arrange
            result.Should().BeTrue();
            sut.Channel.PersonalChannels.Should().BeNullOrEmpty();
            sut.Channel.PrivateChannels.Should().BeNullOrEmpty();
            
            sutMonitor.Should().Raise(nameof(sut.Channel.OnJoinedChannel));
        }

        #region Exit channels tests
        
        [Fact]
        public void Player_cannot_exit_invalid_channel()
        {
            //arrange
            var sut = PlayerTestDataBuilder.Build();
            using var monitor = sut.Channel.Monitor();
            
            //act
            var result = sut.Channel.ExitChannel(null);
            
            //arrange
            result.Should().BeFalse();
            
            monitor.Should().NotRaise(nameof(sut.Channel.OnExitedChannel));
        }
        [Fact]
        public void Player_cannot_exit_channel_that_is_not_in()
        {
            //arrange
            var sut = PlayerTestDataBuilder.Build();
            using var sutMonitor = sut.Channel.Monitor();

            var channel = new ChatChannel(1, "channel 1");
            //act
            var result = sut.Channel.ExitChannel(channel);
            
            //arrange
            result.Should().BeFalse();
         
            sutMonitor.Should().NotRaise(nameof(sut.Channel.OnExitedChannel));
        }

        [Fact]
        public void Player_exits_channel()
        {
            //arrange
            var sut = PlayerTestDataBuilder.Build();
            using var monitor = sut.Channel.Monitor();
            
            var channel = new ChatChannel(1, "channel 1");
            sut.Channel.JoinChannel(channel);
            
            //act
            var result = sut.Channel.ExitChannel(channel);
            
            //arrange
            result.Should().BeTrue();
            monitor.Should().Raise(nameof(sut.Channel.OnExitedChannel));
        }

        #endregion
    }
}