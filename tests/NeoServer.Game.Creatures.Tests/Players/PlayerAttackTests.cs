using System.Net;
using FluentAssertions;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Tests.Helpers;
using NeoServer.Game.World.Models.Tiles;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.Players;

public class PlayerAttackTests
{
    [Fact]
    public void Player_cannot_set_attack_target_when_enemy_is_in_protection_zone()
    {
        //arrange
        var location = new Location(100, 100, 7);
        var ground = MapTestDataBuilder.CreateGround(location);

        var regularTile = new DynamicTile(new Coordinate(100, 100, 7), (TileFlag)TileFlags.None, ground, null, null);
        var protectionZoneTile = new DynamicTile(new Coordinate(100, 100, 7), (TileFlag)TileFlags.ProtectionZone, ground, null, null);
        
        var player = PlayerTestDataBuilder.Build();
        var enemy = MonsterTestDataBuilder.Build();

        protectionZoneTile.AddCreature(enemy);
        regularTile.AddCreature(player);
        
        using var monitor = player.Monitor();

        //act
        player.SetAttackTarget(enemy);

        //assert
        monitor.Should().Raise(nameof(player.OnAttackCanceled));
        player.Attacking.Should().BeFalse();
        player.CurrentTarget.Should().BeNull();
        player.AutoAttackTargetId.Should().Be(0);
        player.IsFollowing.Should().BeFalse();
    }
}