using NeoServer.Game.Tests;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace NeoServer.Game.Chats.Tests
{
    public class ChatChannelTest
    {
        [Fact]
        public void WriteMessage_Sets_User_To_Muted_when_Sending_Multiple_Times()
        {
            var sut = new ChatChannel(1, "channel 1")
            {
                MuteRule = new MuteRule
                {
                    WaitTime = 5,
                    TimeMultiplier = 2,
                    MessagesCount = 5,
                    TimeToBlock = 10
                }
            };

            var player = PlayerTestDataBuilder.BuildPlayer();
            sut.AddUser(player);
            sut.WriteMessage(player, "teste", out var cancelMessage);
            sut.WriteMessage(player, "teste", out cancelMessage);
            sut.WriteMessage(player, "teste", out cancelMessage);
            sut.WriteMessage(player, "teste", out cancelMessage);

            Assert.False(sut.PlayerIsMuted(player, out cancelMessage));

            sut.WriteMessage(player, "teste", out cancelMessage);

            Assert.True(sut.PlayerIsMuted(player, out cancelMessage));

            Thread.Sleep(2000);
            Assert.True(sut.PlayerIsMuted(player, out cancelMessage));
            Thread.Sleep(5000);
            Assert.False(sut.PlayerIsMuted(player, out cancelMessage));
        }

        [Theory]        
        [InlineData(20,3,16)]
        [InlineData(5, 1, 3)]
        [InlineData(5, 5, 0)]
        [InlineData(5, 6, -1)]
        public void RemainingMutedSeconds_Returns_Seconds(ushort mutedForSeconds, int sleepSeconds, int expected)
        {
            var sup = new ChatUser();

            sup.UpdateLastMessage(new MuteRule());
            sup.MutedForSeconds = mutedForSeconds;

            Thread.Sleep(sleepSeconds * 1000);
            Assert.Equal(expected, sup.RemainingMutedSeconds);
        }

    }
}
