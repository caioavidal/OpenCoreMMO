using System;

namespace NeoServer.Server.Helpers
{
    public class StreamArray
    {
        private readonly byte[] stream;

        public StreamArray(byte[] stream)
        {
            this.stream = stream;
        }

        public int Length { get; private set; }

        public void AddUInt32(uint value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }

        public void AddUInt16(ushort value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }

        public byte[] GetStream()
        {
            return stream;
        }

        public void AddByte(byte b)
        {
            WriteBytes(new[] {b});
        }

        /// <summary>
        ///     Add bytes with payload length
        /// </summary>
        /// <param name="bytes"></param>
        public void AddBytes(byte[] bytes)
        {
            foreach (var b in bytes) WriteByte(b);
        }

        private void WriteBytes(byte[] bytes)
        {
            foreach (var b in bytes) WriteByte(b);
        }

        private void WriteByte(byte b)
        {
            stream[Length++] = b;
        }
    }
}