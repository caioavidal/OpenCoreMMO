using System;
using System.Text;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Messages
{
    public class ReadOnlyNetworkMessage : BaseNetworkMessage, IReadOnlyNetworkMessage
    {
        public int BytesRead { get; private set; } = 0;

        public ReadOnlyNetworkMessage(byte[] buffer) => Buffer = buffer;

        protected ReadOnlyNetworkMessage() { }

        public GameIncomingPacketType IncomingPacketType
        {
            get
            {
                return (GameIncomingPacketType)BitConverter.ToInt16(Buffer[0..2]);
            }
        }

        public ushort GetUInt16()
        {
            var to = BytesRead + sizeof(ushort);
            var value = BitConverter.ToUInt16(Buffer[BytesRead..to], 0);
            BytesRead = to;
            return value;

        }
        public uint GetUInt32()
        {
            var to = BytesRead + sizeof(uint);
            var value = BitConverter.ToUInt32(Buffer[BytesRead..to], 0);
            BytesRead = to;
            return value;
        }

        public void SkipBytes(int length)
        {
            if(length + BytesRead > Buffer.Length)
            {
                throw new ArgumentOutOfRangeException("Cannot skip bytes that exceeds the buffer length");
            }
            BytesRead += length;
        }

        private void ResetPosition() => BytesRead = 0;

        public byte GetByte()
        {
            var to = BytesRead + sizeof(byte);
            var value = Buffer[BytesRead..to];
            BytesRead = to;
            return value[0];
        }
        public byte[] GetBytes(int length)
        {
            var to = BytesRead + length;
            var value = Buffer[BytesRead..to];
            BytesRead = to;
            return value;
        }

        /// <summary>
        /// Get string value based on payload length
        /// </summary>
        /// <returns></returns>
        public string GetString()
        {

            var length = GetUInt16();
            var to = BytesRead + length;

            var value = Encoding.UTF8.GetString(Buffer[BytesRead..to]);
            BytesRead = to;

            return value;
        }
    }

}