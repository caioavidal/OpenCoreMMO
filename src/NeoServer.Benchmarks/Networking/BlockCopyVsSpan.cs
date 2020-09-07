using BenchmarkDotNet.Attributes;
using System;

namespace NeoServer.Benchmarks.Networking
{
    [SimpleJob(launchCount: 1)]

    public class BlockCopyVsSpan
    {

        [Benchmark]
        public byte[] AddLength()
        {
            byte[] Buffer = new byte[16394];
            int Length = 100;
            int Cursor = 100;

            var newArray = new byte[16394];
            System.Buffer.BlockCopy(Buffer, 0, newArray, 2, Length);

            var length = BitConverter.GetBytes((ushort)Length);

            for (int i = 0; i < 2; i++)
            {
                newArray[i] = length[i];
            }
            //Length = Length + 2;
            //Cursor += 2;
            Buffer = newArray;

            return Buffer;
        }

        [Benchmark]
        public byte[] AddLengthUsingSpan()
        {
            byte[] Buffer = new byte[16394];
            int Length = 100;
            int Cursor = 100;

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

}
