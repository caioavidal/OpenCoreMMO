using System;
using NeoServer.Loaders.OTB.DataStructures;
using Xunit;

namespace NeoServer.Loaders.Tests.OTB;

public class ReadOnlyMemoryStreamTest
{
    [Fact]
    public void Instance_NullBuffer_NotThrows()
    {
        var instance = new ReadOnlyMemoryStream(null);
        Assert.NotNull(instance);
        Assert.Equal(0, instance.Position);
    }

    [Fact]
    public void Instance_PositionBiggerThan0_ReturnPosition()
    {
        var instance = new ReadOnlyMemoryStream(new byte[10], 5);
        Assert.Equal(5, instance.Position);
    }

    [Fact]
    public void Instance_NegativePosition_Returns()
    {
        var buffer = new ReadOnlyMemory<byte>();
        var sut = new ReadOnlyMemoryStream(buffer, -1);

        Assert.Equal(default, sut.Position);
    }

    [Fact]
    public void Instance_PositionBiggerThanBuffer_()
    {
        var buffer = new ReadOnlyMemory<byte>(new byte[7]);
        var sut = new ReadOnlyMemoryStream(buffer, 15);
        Assert.Equal(default, sut.Position);
    }

    [Fact]
    public void PositionIncrease_WhenReadingBytes()
    {
        var instance = new ReadOnlyMemoryStream(new byte[20]);
        instance.ReadUInt16();
        Assert.Equal(2, instance.Position);
        instance.ReadUInt32();
        Assert.Equal(6, instance.Position);
        instance.ReadByte();
        Assert.Equal(7, instance.Position);
        instance.Skip(7);
        Assert.Equal(14, instance.Position);
        instance.PeakByte();
        Assert.Equal(14, instance.Position);
    }

    [Fact]
    public void WhenReadingBytes_ReturnValue()
    {
        var data = new byte[20]
        {
            1, 4, 5, 7, 2, 7, 9, 2, 5, 2, 8, 3, 5, 8, 5, 3, 8, 4, 8, 5
        };

        var instance = new ReadOnlyMemoryStream(data);

        Assert.Equal(BitConverter.ToUInt16(data, 0), instance.ReadUInt16());

        Assert.Equal(BitConverter.ToUInt32(data, 2), instance.ReadUInt32());

        Assert.Equal(data[6], instance.ReadByte());
        instance.Skip(6);

        Assert.Equal(data[13], instance.PeakByte());
        Assert.Equal(BitConverter.ToUInt32(data, 13), instance.ReadUInt32());
    }

    [Fact]
    public void IsOver_PositionEqualsOrBiggerThanBuffer_ReturnsTrue()
    {
        var buffer = new ReadOnlyMemory<byte>(new byte[7]);
        var instance = new ReadOnlyMemoryStream(buffer);

        instance.Skip(7);

        Assert.True(instance.IsOver);
    }

    [Fact]
    public void IsOver_PositionLessThanBuffer_ReturnsFalse()
    {
        var buffer = new ReadOnlyMemory<byte>(new byte[7]);
        var instance = new ReadOnlyMemoryStream(buffer);

        instance.Skip(5);

        Assert.False(instance.IsOver);
    }
}