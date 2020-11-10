using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using Xunit;

namespace NeoServer.Game.Model.Tests.Structs
{
    public class LocationTest
    {
        [Theory]
        [InlineData(100, 100, 105, 103, 8)]
        [InlineData(105, 103, 100, 100, 8)]
        [InlineData(100, 100, -105, -103, 408)]
        [InlineData(100, 100, -105, 103, 208)]
        [InlineData(100, 0, 0, 100, 200)]
        public void GetSqmDistance_Returns_Sum_Of_Sqm_Distance(int fromX, int fromY, int toX, int toY, ushort total)
        {
            var fromLocation = new Location((ushort)fromX, (ushort)fromY, 7);
            var toLocation = new Location((ushort)toX, (ushort)toY, 7);

            Assert.Equal(total, fromLocation.GetSqmDistance(toLocation));
        }

        [Theory]
        [InlineData(100, 100, 100, 100, Direction.None)]
        [InlineData(100, 100, 101, 100, Direction.East)]
        [InlineData(100, 100, 99, 100, Direction.West)]
        [InlineData(100, 100, 100, 101, Direction.South)]
        [InlineData(100, 100, 100, 99, Direction.North)]
        [InlineData(100, 100, 101, 99, Direction.East)]
        [InlineData(100, 100, 99, 101, Direction.West)]
        [InlineData(100, 100, 102, 101, Direction.East)]
        [InlineData(100, 100, 102, 103, Direction.South)]
        [InlineData(100, 100, 98, 101, Direction.West)]
        [InlineData(100, 100, 102, 97, Direction.North)]

        public void DirectionTo_Returns_TargetDirection(int fromX, int fromY, int toX, int toY, Direction direction)
        {
            var fromLocation = new Location((ushort)fromX, (ushort)fromY, 7);
            var toLocation = new Location((ushort)toX, (ushort)toY, 7);

            Assert.Equal(direction, fromLocation.DirectionTo(toLocation));
        }
    }
}
