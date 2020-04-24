using System;

namespace NeoServer.Server.Helpers
{
    public class StreamArray
    {
        private int cursor;
        private readonly byte[] stream;
        public StreamArray(byte[] stream)
        {
            this.stream = stream;
        }
        public void AddUInt32(uint value) => WriteBytes(BitConverter.GetBytes(value));
        public void AddUInt16(ushort value) => WriteBytes(BitConverter.GetBytes(value));
        public byte[] GetStream() => stream;
        public void AddByte(byte b) => WriteBytes(new[] { b });

        /// <summary>
        /// Add bytes with payload length
        /// </summary>
        /// <param name="bytes"></param>
        public void AddBytes(byte[] bytes)
        {
            foreach (var b in bytes)
            {
                WriteByte(b);
            }
        }

        private void WriteBytes(byte[] bytes)
        {
            foreach (var b in bytes)
            {
                WriteByte(b);
            }
        }
        public int Length => cursor;

        private void WriteByte(byte b)
        {
            stream[cursor++] = b;
        }
    }
}
