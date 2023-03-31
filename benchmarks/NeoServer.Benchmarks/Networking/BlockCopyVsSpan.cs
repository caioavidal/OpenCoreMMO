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
        var Buffer = new byte[16394];
        var Length = 100;
        var Cursor = 100;

        var srcBuffer = Buffer.AsSpan(0, Length);
        var newArray = new byte[Length + 2].AsSpan();

        var lengthBytes = BitConverter.GetBytes((ushort)Length);
        newArray[0] = lengthBytes[0];
        newArray[1] = lengthBytes[1];

        srcBuffer.CopyTo(newArray.Slice(2, Length));

        //Length = Length + 2;
        //Cursor += 2;
        Buffer = newArray.ToArray();

        return Buffer;
    }
}