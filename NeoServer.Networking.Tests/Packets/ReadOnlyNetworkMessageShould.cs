using NeoServer.Networking.Packets.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NeoServer.Networking.Tests.Packets
{
    public class ReadOnlyNetworkMessageShould : IClassFixture<ReadOnlyNetworkMessageFixture>
    {
        private readonly ReadOnlyNetworkMessageFixture _fixture;
        public ReadOnlyNetworkMessageShould(ReadOnlyNetworkMessageFixture fixture)
        {
            _fixture = fixture;
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
            var data = "hello world";

            var payload = Encoding.ASCII.GetBytes(data);
            var payloadLength = BitConverter.GetBytes((ushort)payload.Length);

            var dataBytes = new byte[13];
            payloadLength.CopyTo(dataBytes, 0);

            payload.CopyTo(dataBytes, 2);

            var sup = new ReadOnlyNetworkMessage(dataBytes);

            var expected = "hello world";
            Assert.Equal(expected, sup.GetString());
        }


    }
}
