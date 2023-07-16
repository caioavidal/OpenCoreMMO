using System;
using BenchmarkDotNet.Attributes;

namespace NeoServer.Benchmarks.Networking;

[SimpleJob(1)]
public class BlockCopyVsSpan
{
    [Benchmark]
    public byte[] AddLength()
    {
        var buffer = new byte[16394];
        const int count = 100;

        var newArray = new byte[16394];
        Buffer.BlockCopy(buffer, 0, newArray, 2, count);

        var length = BitConverter.GetBytes((ushort)count);

        for (var i = 0; i < 2; i++) newArray[i] = length[i];
        //Length = Length + 2;
        //Cursor += 2;
        buffer = newArray;

        return buffer;
    }

    [Benchmark]
    public byte[] AddLengthUsingSpan()
    {
        var buffer = new byte[16394];
        var length = 100;

        var srcBuffer = buffer.AsSpan(0, length);
        var newArray = new byte[length + 2].AsSpan();

        var lengthBytes = BitConverter.GetBytes((ushort)length);
        newArray[0] = lengthBytes[0];
        newArray[1] = lengthBytes[1];

        srcBuffer.CopyTo(newArray.Slice(2, length));

        //Length = Length + 2;
        //Cursor += 2;
        buffer = newArray.ToArray();

        return buffer;
    }
}