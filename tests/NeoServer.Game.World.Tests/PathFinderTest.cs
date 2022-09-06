using System.Runtime.Intrinsics.X86;
using FluentAssertions;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures;
using NeoServer.Game.Tests.Helpers;
using NeoServer.Game.Tests.Helpers.Map;
using NeoServer.Game.World.Algorithms;
using NeoServer.Game.World.Models.Tiles;
using Xunit;

namespace NeoServer.Game.World.Tests;

public class PathFinderTest
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Path_finder_gets_all_directions_to_target(bool fullPathSearch)
    {
        //arrange
        var map = MapTestDataBuilder.Build(32089, 32095, 32202, 32207, 7, 7, addGround: true);
        
        var player = PlayerTestDataBuilder.Build(1);
        
        ((DynamicTile)map[32090, 32202, 7]).AddCreature(player);
        
        var fpp = new FindPathParams
        {
            AllowDiagonal = true,
            ClearSight = true,
            KeepDistance = false,
            OneStep = false,
            FullPathSearch = fullPathSearch,
            MaxSearchDist = 12,
            MaxTargetDist = 1,
            MinTargetDist = 1
        };

        var tileEnterRule = PlayerEnterTileRule.Rule;
        
        var sut = new AStarTibia();
        //act
        var result = sut.GetPathMatching(map, player, new Location(32094, 32205, 7), fpp, tileEnterRule, out var directions);

        //assert
        result.Should().BeTrue();
        directions.Should().BeEquivalentTo(new[]
        {
            Direction.East, Direction.South, Direction.East, Direction.South, Direction.East
        });
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Path_finder_gets_no_directions_when_closed_to_target(bool fullPathSearch)
    {
        //arrange
        var map = MapTestDataBuilder.Build(32089, 32095, 32202, 32207, 7, 7, addGround: true);
        
        var player = PlayerTestDataBuilder.Build(1);
        
        ((DynamicTile)map[32093, 32204, 7]).AddCreature(player);
        
        var fpp = new FindPathParams
        {
            AllowDiagonal = true,
            ClearSight = true,
            KeepDistance = false,
            OneStep = false,
            FullPathSearch = fullPathSearch,
            MaxSearchDist = 12,
            MaxTargetDist = 1,
            MinTargetDist = 1
        };

        var tileEnterRule = PlayerEnterTileRule.Rule;
        
        var sut = new AStarTibia();
        //act
        var result = sut.GetPathMatching(map, player, new Location(32094, 32205, 7), fpp, tileEnterRule, out var directions);

        //assert
        result.Should().BeTrue();
        directions.Should().BeEmpty();
    }
}