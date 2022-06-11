using FluentAssertions;
using NeoServer.Game.Common.Helpers;
using Xunit;

namespace NeoServer.Game.Common.Tests.Effects;
public class MatrixExtensionsTest
{
    [Fact]
    public void Rotate_area()
    {
        //arrange
        byte[,] area =
        {
            { 1, 1, 1, 1, 1 },
            { 0, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 0 },
            { 0, 0, 3, 0, 0 },
            { 0, 0, 0, 0, 0 }
        };
        
        byte[,] expected =
        {
            { 1, 0, 0, 0, 0 },
            { 1, 1, 1, 0, 0 },
            { 1, 1, 1, 3, 0 },
            { 1, 1, 1, 0, 0 },
            { 1, 0, 0, 0, 0 }
        };
        
        //act
        var result = area.Rotate();
        
        //assert
        result.Should().BeEquivalentTo(expected);
    }
}