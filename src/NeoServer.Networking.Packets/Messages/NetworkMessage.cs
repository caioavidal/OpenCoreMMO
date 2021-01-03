using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Items;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Security;
using System;
using System.Text;

namespace NeoServer.Networking.Packets
{
    /// <summary>
    /// Contains all the methods to handle incoming and outgoing message from/to client
    /// </summary>
    public class NetworkMessage : ReadOnlyNetworkMessage, INetworkMessage
    {
        private int Cursor;
        public NetworkMessage(byte[] buffer, int length) : base(buffer, length)
        {
            Length = length;
            Cursor = length;
        }
        public NetworkMessage(int length = 0) : base(new byte[16394], length)
        {
            Length = length;
            Cursor = length;
        }

        /// <summary>
        /// Inserts a location point on the buffer
        /// </summary>
        /// <param name="location"></param>
        public void AddLocation(Location location)
        {
            AddUInt16(location.X);
            AddUInt16(location.Y);
            AddByte(location.Z);
        }

        /// <summary>
        /// Inserts a string on buffer. This method put the string's length in front of the value itself
        /// Ex: the string "hello world" will be inserted as 0x0b 0 + string bytes
        /// </summary>
        /// <param name="value"></param>
        public void AddString(string value)
        {
            AddUInt16((ushort)value.Length);
            WriteBytes(Encoding.UTF8.GetBytes(value));
        }

        /// <summary>
        /// Inserts item object into the buffer.
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(IItem item)
        {

            if (item == null)
            {
                //todo log
                return;
            }

            AddBytes(item.GetRaw().ToArray());
        }

        /// <summary>
        /// Adds uint value (4 bytes) to buffer
        /// </summary>
        /// <param name="value"></param>
        public void AddUInt32(uint value) => WriteBytes(BitConverter.GetBytes(value));

        /// <summary>
        /// Adds ushort (2 bytes) to buffer
        /// </summary>
        /// <param name="value"></param>
        public void AddUInt16(ushort value) => WriteBytes(BitConverter.GetBytes(value));

        /// <summary>
        /// Adds a byte value to buffer
        /// </summary>
        /// <param name="b"></param>
        public void AddByte(byte b) => WriteBytes(new[] { b });

        /// <summary>
        /// Adds a array of bytes to buffer
        /// </summary>
        /// <param name="bytes"></param>
        public void AddBytes(byte[] bytes)
        {
            foreach (var b in bytes)
            {
                WriteByte(b);
            }
        }

        /// <summary>
        /// Adds padding right bytes (0x33) to buffer
        /// </summary>
        /// <param name="count">how many times the padding will be added</param>
        public void AddPaddingBytes(int count) => WriteBytes(0x33, count);

        private void WriteBytes(byte[] bytes)
        {
            foreach (var b in bytes)
            {
                WriteByte(b);
            }
        }
        private void WriteBytes(byte b, int times)
        {
            for (int i = 0; i < times; i++)
            {
                WriteByte(b);
            }
        }

        private void WriteByte(byte b)
        {
            Length++;
            Buffer[Cursor++] = b;
        }

        /// <summary>
        /// Add header bytes and return the whole packet
        /// </summary>
        /// <param name="addChecksum"></param>
        /// <returns></returns>
        public byte[] AddHeader(bool addChecksum = true)
        {

            var newArray = new byte[Length + 6];

            var header = GetHeader();

            System.Buffer.BlockCopy(Buffer, 0, newArray, 6, Length);
            System.Buffer.BlockCopy(header, 0, newArray, 0, 6);

            return newArray;
        }

        /// <summary>
        /// Add payload length to the buffer
        /// The ushort bytes will be added in front of buffer
        /// </summary>
        public void AddLength()
        {

            var srcBuffer = Buffer.AsSpan(0, Length);
            var newArray = new byte[Length + 50].AsSpan(); //added 50 for xtea padding

            var lengthBytes = BitConverter.GetBytes((ushort)Length);
            newArray[0] = lengthBytes[0];
            newArray[1] = lengthBytes[1];

            srcBuffer.CopyTo(newArray.Slice(2, Length));

            Length = Length + 2;
            Cursor += 2;
            Buffer = newArray.ToArray();

        }

        private byte[] GetHeader(bool addChecksum = true)
        {
            var checkSumBytes = new byte[4];
            if (addChecksum)
            {
                var adlerChecksum = AdlerChecksum.Checksum(Buffer, 0, Length); //todo: 6 is the header length
                checkSumBytes = BitConverter.GetBytes(adlerChecksum);
            }
            var lengthInBytes = BitConverter.GetBytes((ushort)(Length + checkSumBytes.Length));

            var header = new byte[6];

            System.Buffer.BlockCopy(lengthInBytes, 0, header, 0, 2);
            System.Buffer.BlockCopy(checkSumBytes, 0, header, 2, 4);
            return header;

        }

    }
}
