namespace NeoServer.Networking.Packets
{
    using NeoServer.Server.Contracts.Network;
    using NeoServer.Server.Security;
    using System;
    using System.Linq;
    using System.Text;

    public class NetworkMessage : BaseNetworkMessage, INetworkMessage
    {
        private int Position;

        public NetworkMessage()
        {
            Buffer = new byte[1024];            
        }

        public void AddString(string value)
        {
            AddUInt16((ushort)value.Length);
            AddBytes(System.Text.Encoding.UTF8.GetBytes(value));
        }

        public void AddUInt32(uint value) => AddBytes(BitConverter.GetBytes(value));
        public void AddUInt16(ushort value) => AddBytes(BitConverter.GetBytes(value));

        public void AddUInt8(sbyte value) => AddBytes(BitConverter.GetBytes(value));
        public void AddByte(byte b) => AddBytes(new[] { b });

        public void AddPaddingBytes(int count) => AddBytes(0x33, count);

        private void AddHeader(bool addChecksum)
        {
            var checkSumBytes = new byte[4];
            if (addChecksum)
            {
                var adlerChecksum = AdlerChecksum.Checksum(Buffer, 0, BufferLength); //todo: 6 is the header length
                checkSumBytes = BitConverter.GetBytes(adlerChecksum);
            }
            var lengthInBytes = BitConverter.GetBytes((ushort)(BufferLength + checkSumBytes.Length));

            Header = lengthInBytes.Concat(checkSumBytes).ToArray();
        }


        /// <summary>
        /// Get network message with the body buffer within header (length and adler)
        /// </summary>
        /// <returns></returns>
        public byte[] GetMessageInBytes(bool addHeader = true)
        {
            if (addHeader)
            {
                AddHeader(true);
                return Header.Concat(Buffer).ToArray();
            };
            return Buffer.ToArray();
        }
        private byte[] GetLengthBytes() => BitConverter.GetBytes((ushort)BufferLength);

        private void AddLengthToHeader()
        {
            var length = GetLengthBytes();

            for (int i = 0; i < length.Length; i++)
            {
                WriteByte(length[i], i);
            }
        }



        private void AddBytes(byte[] bytes)
        {
            foreach (var b in bytes)
            {
                WriteByte(b);
            }
        }
        private void AddBytes(byte b, int times)
        {
            for (int i = 0; i < times; i++)
            {
                WriteByte(b);
            }
        }

        private void WriteByte(byte b, int position)
        {
            BufferLength++;
            Buffer[position] = b;
        }
        private void WriteByte(byte b)
        {
            BufferLength++;
            Buffer[Position++] = b;
        }


    }
}
