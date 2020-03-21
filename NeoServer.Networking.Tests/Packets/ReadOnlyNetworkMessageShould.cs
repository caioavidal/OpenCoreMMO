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
            var dataBytes = BitConverter.GetBytes(data);
            var sup = new ReadOnlyNetworkMessage(dataBytes, dataBytes.Length);

            var expected = new byte[] { 141, 54 };
            Assert.Equal(BitConverter.ToUInt16(expected, 0), sup.GetUInt16());
        }

        [Fact]
        public void Return_Values()
        {
            var data = "1652365658\n\0000006987451230246545648945646";
            var dataBytes = Encoding.ASCII.GetBytes(data);
            var sup = new ReadOnlyNetworkMessage(dataBytes, dataBytes.Length);

            Assert.Equal(BitConverter.ToUInt16(new byte[] { 49, 54 }, 0), sup.GetUInt16());

            Assert.Equal(BitConverter.ToUInt32(new byte[] { 53, 50, 51, 54 }, 0), sup.GetUInt32());

            sup.SkipBytes(3);

            Assert.Equal((byte)56, sup.GetByte());

            var s = sup.GetString();

            Assert.Equal("0000069874", s);
        }

        [Fact]
        public void Increase_BytesRead()
        {
            var data = "1652365658\n\0000006987451230246545648945646";
            var dataBytes = Encoding.ASCII.GetBytes(data);

            var sup = new ReadOnlyNetworkMessage(dataBytes, dataBytes.Length);

            sup.GetUInt16();
            Assert.Equal(2, sup.BytesRead);

            sup.GetUInt32();
            Assert.Equal(6, sup.BytesRead);

            sup.GetByte();
            Assert.Equal(7, sup.BytesRead);

            sup.SkipBytes(3);
            Assert.Equal(10, sup.BytesRead);

            sup.GetString();
            Assert.Equal(22, sup.BytesRead);

        }

        [Fact]
        public void Return_UInt()
        {
            var data = 1652365;
            var dataBytes = BitConverter.GetBytes(data);

            var sup = new ReadOnlyNetworkMessage(dataBytes, dataBytes.Length);
            Assert.Equal((uint)1652365, sup.GetUInt32());
        }

        [Fact]
        public void SkipBytes()
        {
            var data = 1652365;
            var dataBytes = BitConverter.GetBytes(data);

            var sup = new ReadOnlyNetworkMessage(dataBytes, dataBytes.Length);

            sup.SkipBytes(2);

            Assert.Equal(2, sup.BytesRead);
        }

        [Fact]
        public void ThrowException_SkipBytes()
        {
            var data = 1652365;
            var dataBytes = BitConverter.GetBytes(data);

            var sup = new ReadOnlyNetworkMessage(dataBytes,dataBytes.Length);

            Assert.Throws<ArgumentOutOfRangeException>(() => sup.SkipBytes(20));
        }

        [Fact]
        public void GetByte()
        {
            var data = 1652365;
            var dataBytes = BitConverter.GetBytes(data);

            var sup = new ReadOnlyNetworkMessage(dataBytes, dataBytes.Length);

            var expected = (byte)141;
            Assert.Equal(expected, sup.GetByte());
        }

        [Fact]
        public void GetBytes()
        {
            var data = 1652365;
            var dataBytes = BitConverter.GetBytes(data);

            var sup = new ReadOnlyNetworkMessage(dataBytes,dataBytes.Length);

            var expected = new byte[] { 141, 54, 25 };
            Assert.Equal(expected, sup.GetBytes(3));
        }

        [Fact]
        public void GetString()
        {
            var data = "\a\0hello world";
            var dataBytes = Encoding.ASCII.GetBytes(data);
            var sup = new ReadOnlyNetworkMessage(dataBytes, dataBytes.Length);

            var expected = "hello w";
            Assert.Equal(expected, sup.GetString());
        }


    }
}
