using System.Net;
using FluentAssertions;
using NeoServer.Game.Common;
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
        var enemy = PlayerTestDataBuilder.Build();

        protectionZoneTile.AddCreature(enemy);
        regularTile.AddCreature(player);
        
        using var monitor = player.Monitor();

        //act
        var result = player.SetAttackTarget(enemy);

        //assert
        result.Error.Should().Be(InvalidOperation.CannotAttackPersonInProtectionZone);
        
        monitor.Should().Raise(nameof(player.OnAttackCanceled));
        player.Attacking.Should().BeFalse();
        player.CurrentTarget.Should().BeNull();
        player.AutoAttackTargetId.Should().Be(0);
        player.IsFollowing.Should().BeFalse();
    }
    
    [Fact]
    public void Player_cannot_set_attack_target_while_in_protection_zone()
    {
        //arrange
        var location = new Location(100, 100, 7);
        var ground = MapTestDataBuilder.CreateGround(location);

        var regularTile = new DynamicTile(new Coordinate(100, 100, 7), (TileFlag)TileFlags.None, ground, null, null);
        var protectionZoneTile = new DynamicTile(new Coordinate(100, 100, 7), (TileFlag)TileFlags.ProtectionZone, ground, null, null);
        
        var player = PlayerTestDataBuilder.Build();
        var enemy = PlayerTestDataBuilder.Build();

        protectionZoneTile.AddCreature(player);
        regularTile.AddCreature(enemy);
        
        using var monitor = player.Monitor();

        //act
        var result = player.SetAttackTarget(enemy);

        //assert
        result.Error.Should().Be(InvalidOperation.CannotAttackWhileInProtectionZone);
        
        monitor.Should().Raise(nameof(player.OnAttackCanceled));
        player.Attacking.Should().BeFalse();
        player.CurrentTarget.Should().BeNull();
        player.AutoAttackTargetId.Should().Be(0);
        player.IsFollowing.Should().BeFalse();
    }
    
    [Fact]
    public void Player_cannot_attack_when_enemy_goes_to_protection_zone()
    {
        //arrange
        var location = new Location(100, 100, 7);
        var ground = MapTestDataBuilder.CreateGround(location);

        var regularTile = new DynamicTile(new Coordinate(100, 100, 7), (TileFlag)TileFlags.None, ground, null, null);
        var regularTile2 = new DynamicTile(new Coordinate(100, 101, 7), (TileFlag)TileFlags.None, ground, null, null);

        var protectionZoneTile = new DynamicTile(new Coordinate(101, 100, 7), (TileFlag)TileFlags.ProtectionZone, ground, null, null);
        
        var player = PlayerTestDataBuilder.Build();
        var enemy = PlayerTestDataBuilder.Build();

        regularTile.AddCreature(player);
        regularTile2.AddCreature(enemy);
        
        using var monitor = player.Monitor();
        
        player.SetAttackTarget(enemy);
        
        regularTile2.RemoveCreature(enemy, out _);
        protectionZoneTile.AddCreature(enemy);

        //act

        var result = player.Attack(enemy);

        //assert
        result.Error.Should().Be(InvalidOperation.CannotAttackPersonInProtectionZone);
        
        monitor.Should().Raise(nameof(player.OnStoppedAttack));
        
        player.Attacking.Should().BeFalse();
        player.CurrentTarget.Should().BeNull();
        player.AutoAttackTargetId.Should().Be(0);
        player.IsFollowing.Should().BeFalse();
    }
    
    [Fact]
    public void Player_cannot_attack_when_goes_to_protection_zone()
    {
        //arrange
        var location = new Location(100, 100, 7);
        var ground = MapTestDataBuilder.CreateGround(location);

        var regularTile = new DynamicTile(new Coordinate(100, 100, 7), (TileFlag)TileFlags.None, ground, null, null);
        var regularTile2 = new DynamicTile(new Coordinate(100, 101, 7), (TileFlag)TileFlags.None, ground, null, null);

        var protectionZoneTile = new DynamicTile(new Coordinate(101, 100, 7), (TileFlag)TileFlags.ProtectionZone, ground, null, null);
        
        var player = PlayerTestDataBuilder.Build();
        var enemy = PlayerTestDataBuilder.Build();

        regularTile.AddCreature(player);
        regularTile2.AddCreature(enemy);
        
        using var monitor = player.Monitor();
        
        player.SetAttackTarget(enemy);
        
        regularTile.RemoveCreature(player, out _);
        protectionZoneTile.AddCreature(player);

        //act
        var result = player.Attack(enemy);

        //assert
        result.Error.Should().Be(InvalidOperation.CannotAttackWhileInProtectionZone);
        
        monitor.Should().Raise(nameof(player.OnStoppedAttack));
        
        player.Attacking.Should().BeFalse();
        player.CurrentTarget.Should().BeNull();
        player.AutoAttackTargetId.Should().Be(0);
        player.IsFollowing.Should().BeFalse();
    }
}