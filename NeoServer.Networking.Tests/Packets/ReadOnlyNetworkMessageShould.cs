using NeoServer.Networking.Packets.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NeoServer.Networking.Tests.Packets
{
    public class ReadOnlyNetworkMessageShould
    {
        public ReadOnlyNetworkMessageShould()
        {
           
        }

        [Fact]
        public void Return_Ushort()
        {
            var data = 1652365;
            var sup = new ReadOnlyNetworkMessage(BitConverter.GetBytes(data));

            var expected = new byte[] { 141, 54 };
            Assert.Equal(BitConverter.ToUInt16(expected, 0), sup.GetUInt16());
        }

        [Fact]
        public void Return_Values()
        {
            var data = "1652365658\n\0000006987451230246545648945646";
            var sup = new ReadOnlyNetworkMessage(Encoding.ASCII.GetBytes(data));

            Assert.Equal(BitConverter.ToUInt16(new byte[] { 49, 54 }, 0), sup.GetUInt16());

            Assert.Equal(BitConverter.ToUInt32(new byte[] { 53, 50, 51, 54 }, 0), sup.GetUInt32());

            sup.SkipBytes(3);

            Assert.Equal((byte)56, sup.GetByte());

            var s = sup.GetString();

            Assert.Equal(s, "0000069874");
        }

        [Fact]
        public void Increase_BytesRead()
        {
            var data = "1652365658\n\0000006987451230246545648945646";
            var sup = new ReadOnlyNetworkMessage(Encoding.ASCII.GetBytes(data));

            sup.GetUInt16();
            Assert.Equal(sup.BytesRead, 2);

            sup.GetUInt32();
            Assert.Equal(sup.BytesRead, 6);

            sup.GetByte();
            Assert.Equal(sup.BytesRead, 7);

            sup.SkipBytes(3);
            Assert.Equal(sup.BytesRead, 10);

            sup.GetString();
            Assert.Equal(sup.BytesRead, 22);

        }

        [Fact]
        public void Return_UInt()
        {
            var data = 1652365;
            var sup = new ReadOnlyNetworkMessage(BitConverter.GetBytes(data));

            var expected = new int[] { 141, 54 };
            Assert.Equal((uint)1652365, sup.GetUInt32());
        }

        [Fact]
        public void SkipBytes()
        {
            var data = 1652365;
            var sup = new ReadOnlyNetworkMessage(BitConverter.GetBytes(data));

            sup.SkipBytes(2);

            Assert.Equal(2, sup.BytesRead);
        }

        [Fact]
        public void ThrowException_SkipBytes()
        {
            var data = 1652365;
            var sup = new ReadOnlyNetworkMessage(BitConverter.GetBytes(data));

            Assert.Throws<ArgumentOutOfRangeException>(()=>sup.SkipBytes(20));
        }

        [Fact]
        public void GetByte()
        {
            var data = 1652365;
            var sup = new ReadOnlyNetworkMessage(BitConverter.GetBytes(data));

            var expected = (byte)141;
            Assert.Equal(expected, sup.GetByte());
        }

        [Fact]
        public void GetBytes()
        {
            var data = 1652365;
            var sup = new ReadOnlyNetworkMessage(BitConverter.GetBytes(data));

            var expected = new byte[] { 141, 54, 25 };
            Assert.Equal(expected, sup.GetBytes(3));
        }

        [Fact]
        public void GetString()
        {
            var data = "\a\0hello world";

            var sup = new ReadOnlyNetworkMessage(Encoding.ASCII.GetBytes(data));

            var expected = "hello w";
            Assert.Equal(expected, sup.GetString());
        }


    }
}
