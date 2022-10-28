using FluentAssertions;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Tests.Helpers;
using NeoServer.Game.Tests.Helpers.Player;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.Players;

public class PlayerFollowTests
{
    [Fact]
    public void Player_does_not_follow_dead_creature()
    {
        //arrange
        var sut = PlayerTestDataBuilder.Build(hp: 200);
        var enemy = MonsterTestDataBuilder.Build(1);
        using var monitor = sut.Monitor();

        var fpp = new FindPathParams(true, true, true, false, 12, 0, 12, false);

        sut.Follow(enemy, fpp);
        enemy.ReceiveAttack(sut, new CombatDamage(200, DamageType.Melee));

        //act
        sut.Follow(enemy, fpp);

        //assert
        sut.IsFollowing.Should().BeFalse();
        sut.Following.Should().BeNull();
        sut.HasNextStep.Should().BeFalse();
        monitor.Should().Raise(nameof(sut.OnStoppedWalking));
    }
}