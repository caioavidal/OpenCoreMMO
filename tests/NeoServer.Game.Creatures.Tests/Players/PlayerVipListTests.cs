using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Creatures.Model.Players;
using NeoServer.Game.Tests.Helpers;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.Players
{
    public class PlayerVipListTests
    {
        [Fact]
        public void LoadVipList_Null_Do_Nothing()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100);

            var called = false;
            sut.OnLoadedVipList += (_, _) =>
            {
                called = true;
            };
            sut.LoadVipList(null);

            Assert.Empty(sut.VipList);
            Assert.False(called);
        }

        [Fact]
        public void LoadVipList_Empty_CallLoadedEvent()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100);

            var called = false;
            sut.OnLoadedVipList += (_, _) => { called = true; };

            sut.LoadVipList(Array.Empty<(uint, string)>().AsEnumerable());

            Assert.Empty(sut.VipList);
            Assert.True(called);
        }

        [Fact]
        public void LoadVipList_2DuplicatedItems_FillWith1EntryVipList_CallLoadedEvent()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100);
            var vipToAdd = new List<(uint, string)>()
            {
                (1,"test"),
                (1,"test"),
            };
            var called = false;
            sut.OnLoadedVipList += (_, _) =>
            {
                called = true;
            };

            sut.LoadVipList(vipToAdd);

            Assert.Single(sut.VipList);
            Assert.Equal((uint)1, sut.VipList.FirstOrDefault());
            Assert.True(called);
        }

        [Fact]
        public void LoadVipList_2Items_FillVipList_CallLoadedEventSendingThem()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(hp: 100);
            var vipToAdd = new List<(uint, string)>()
            {
                (1,"test"),
                (2,"test 2"),
            };
            var called = false;
            IEnumerable<(uint, string)> vipSentEvent = null;
            sut.OnLoadedVipList += (_, viplist) =>
            {
                called = true;
                vipSentEvent = viplist;
            };

            sut.LoadVipList(vipToAdd);

            sut.VipList.Should().Contain(1);
            sut.VipList.Should().Contain(2);
            sut.VipList.Should().HaveCount(2);
            vipSentEvent.Should().Contain((1, "test"));
            vipSentEvent.Should().Contain((2, "test 2"));
            vipSentEvent.Should().HaveCount(2);
            Assert.True(called);
        }

        [Fact]
        public void HasInVipList_EmptyVipList_ReturnsFalse()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer();
            var result = sut.HasInVipList(1);
            result.Should().BeFalse();
        }

        [Fact]
        public void HasInVipList_ItemIsNotInVipList_ReturnsFalse()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer();
            sut.LoadVipList(new (uint, string)[]{(1,"test")});
            var result = sut.HasInVipList(2);
            result.Should().BeFalse();
        }
        [Fact]
        public void HasInVipList_ItemInVipList_ReturnsTrue()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer();
            sut.LoadVipList(new (uint, string)[] { (1, "test") });
            var result = sut.HasInVipList(1);
            result.Should().BeTrue();
        }

        [Fact]
        public void AddToVip_Null_DoNothing()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer();
            var result = sut.AddToVip(null);

            var eventCalled = false;
            sut.OnAddedToVipList += (_,_,_) =>
            {
                eventCalled = true;
            };
            result.Should().BeFalse();
            sut.VipList.Should().BeEmpty();
            eventCalled.Should().BeFalse();
        }

        [Fact]
        public void AddToVip_PlayerAlreadyOnVip_ReturnsFalse()
        {
            var sut = PlayerTestDataBuilder.BuildPlayer(id:1);
            var player = PlayerTestDataBuilder.BuildPlayer(id: 2);

            sut.LoadVipList(new (uint, string)[]{(2,"player1")});
            var result = sut.AddToVip(player);

            var eventCalled = false;
            sut.OnAddedToVipList += (_, _, _) =>
            {
                eventCalled = true;
            };
            result.Should().BeFalse();
            sut.VipList.Should().ContainSingle();
            eventCalled.Should().BeFalse();
        }
    }
}
