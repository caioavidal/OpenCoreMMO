using System;
using System.Runtime.InteropServices;
using System.Text;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Messages
{
    public class ReadOnlyNetworkMessage : BaseNetworkMessage, IReadOnlyNetworkMessage
    {
        public int BytesRead { get; private set; } = 0;

        public ReadOnlyNetworkMessage(byte[] buffer) : base(buffer) { }

        private void IncreaseByteRead(int length) => BytesRead += length;

        public static int SizeOf<T>() where T : struct => Marshal.SizeOf(default(T));


        private T Convert<T>(Func<byte[], int, T> converter) where T : struct
        {
            var result = converter(Buffer, BytesRead);
            IncreaseByteRead(SizeOf<T>());
            return result;
        }

        private T Convert<T>(Func<byte[], int, T> converter, int length)
        {
            var result = converter(Buffer, BytesRead);
            IncreaseByteRead(length);
            return result;
        }

        public GameIncomingPacketType IncomingPacketType
        {
            get
            {
                return (GameIncomingPacketType) Buffer[6];
            }
        }

        public ushort GetUInt16() => Convert(BitConverter.ToUInt16);

        public uint GetUInt32() => Convert(BitConverter.ToUInt32);

        public void SkipBytes(int length)
        {
            if (length + BytesRead > Buffer.Length)
            {
                throw new ArgumentOutOfRangeException("Cannot skip bytes that exceeds the buffer length");
            }
            IncreaseByteRead(length);
        }

        public byte GetByte() => Convert((buffer, index) => Buffer[BytesRead]);

        public byte[] GetBytes(int length) =>
            Convert((buffer, index) =>
            {
                var to = BytesRead + length;
                return Buffer[BytesRead..to];
            }, length);

        /// <summary>
        /// Get string value based on payload length
        /// </summary>
        /// <returns></returns>
        public string GetString()
        {

            var length = GetUInt16();

            return Convert((buffer, index) =>
            {
                return Encoding.UTF8.GetString(Buffer, BytesRead, length);
            }, length);
        }


    }

}