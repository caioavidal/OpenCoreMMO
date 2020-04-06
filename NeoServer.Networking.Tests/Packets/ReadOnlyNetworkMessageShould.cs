using NeoServer.Networking.Packets.Messages;
using NeoServer.Server.Contracts.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
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

            var sup = new ReadOnlyNetworkMessage(dataBytes, dataBytes.Length);

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

            var sup = new ReadOnlyNetworkMessage(dataBytes, dataBytes.Length);

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

        [Fact]
        public void ReturnBytes_When_GetMessageInBytes()
        {
            var data = "\a\0hello world";
            var dataBytes = Encoding.ASCII.GetBytes(data);
            var sup = new ReadOnlyNetworkMessage(dataBytes, dataBytes.Length);

            var expected = dataBytes;

            Assert.Equal(expected, sup.GetMessageInBytes());
        }
        [Fact]
        public void ReturnEntireBuffer_When_Length_Equals_0_GetMessageInBytes()
        {
            var data = "\a\0hello world";
            var dataBytes = Encoding.ASCII.GetBytes(data);
            var sup = new ReadOnlyNetworkMessage(dataBytes, 0);

            var expected = dataBytes;

            Assert.Equal(expected, sup.GetMessageInBytes());
        }

        [Fact]
        public void ReturnSlicedBuffer_When_Length_NotEquals_0_GetMessageInBytes()
        {
            var data = "\a\0hello world";
            var dataBytes = Encoding.ASCII.GetBytes(data);
            var length = 3;
            var sup = new ReadOnlyNetworkMessage(dataBytes, length);

            var expected = dataBytes.Take(length);

            Assert.Equal(expected, sup.GetMessageInBytes());
        }

        [Fact]
        public void ThrowArgumentExpecetion_When_Length_Less_Than_0_GetMessageInBytes()
        {
            var length = -1;
            var sup = new ReadOnlyNetworkMessage(new byte[4], length);

            Assert.Throws<ArgumentOutOfRangeException>(sup.GetMessageInBytes);
        }

        [Fact]
        public void Return_None_When_IncomingPacket_Is_Not_Setted()
        {

            var sup = new ReadOnlyNetworkMessage(new byte[4], 4);

            Assert.Equal(GameIncomingPacketType.None, sup.IncomingPacket);
        }

        [Fact]
        public void Return_IncomingPacket_When_Setted()
        {
            var data = new byte[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0x01 };

            var sup = new ReadOnlyNetworkMessage(data, 9);

            sup.GetIncomingPacketType(true);

            Assert.Equal(GameIncomingPacketType.PlayerLoginRequest, sup.IncomingPacket);
        }

        [Fact]
        public void Return_IncomingPacket_When_GetIncomingPacketType_Authenticated()
        {
            var data = new byte[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0x14 };

            var sup = new ReadOnlyNetworkMessage(data, 9);

            Assert.Equal(GameIncomingPacketType.PlayerLogOut, sup.GetIncomingPacketType(true));
        }
        [Fact]
        public void Return_IncomingPacket_When_GetIncomingPacketType_Not_Authenticated()
        {
            var data = new byte[9] { 0, 0, 0, 0, 0, 0, 0x14, 0, 0 };

            var sup = new ReadOnlyNetworkMessage(data, 9);

            Assert.Equal(GameIncomingPacketType.PlayerLogOut, sup.GetIncomingPacketType(false));
        }

        [Fact]
        public void ThrowException_When_Buffer_Less_Than_9_Bytes_GetIncomingPacketType_Is_Authenticated()
        {
            var data = new byte[3] { 0, 0, 0 };

            var sup = new ReadOnlyNetworkMessage(data, 3);

            Assert.Throws<ArgumentOutOfRangeException>(() => sup.GetIncomingPacketType(false));
        }
        [Fact]
        public void ThrowException_When_Buffer_Less_Than_6_Bytes_GetIncomingPacketType_Is_Not_Authenticated()
        {
            var data = new byte[3] { 0, 0, 0 };

            var sup = new ReadOnlyNetworkMessage(data, 3);

            Assert.Throws<ArgumentOutOfRangeException>(() => sup.GetIncomingPacketType(false));
        }

        [Fact]
        public void ThrowException_When_Buffer_Is_Null_Constructor()
        {
            Assert.Throws<NullReferenceException>(() => new ReadOnlyNetworkMessage(null, 3));
        }

        [Fact]
        public void ThrowException_When_Length_Less_Than_0_Resize()
        {
            var sup = new ReadOnlyNetworkMessage(new byte[3], 3);
            Assert.Throws<ArgumentOutOfRangeException>(() => sup.Resize(-1));
        }

        [Fact]
        public void Return_Set_Length_And_BytesRead_Resize()
        {
            var sup = new ReadOnlyNetworkMessage(new byte[3], 3);

            sup.Resize(5);
            Assert.Equal(5, sup.Length);
            Assert.Equal(0, sup.BytesRead);
        }
    }
}
