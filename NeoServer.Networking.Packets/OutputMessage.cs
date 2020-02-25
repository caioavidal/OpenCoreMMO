using System;
using System.Net.Sockets;
using NeoServer.Server.Security;

namespace NeoServer.Networking.Packets
{
    public class OutputMessage
    {
        public byte[] Buffer { get; private set; } = new byte[24590];
        public int HeaderLength { get; private set; }
        public int Length { get; private set; } = 0;

        private int _position;

        // public void Send(Socket socket)
        // {
        //     LoginListener.Send(socket, this);

        // }

        public OutputMessage(int headerLength)
        {
            HeaderLength = headerLength;
            _position = HeaderLength;
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

        public void AddHeader(bool addChecksum)
        {
            if (addChecksum)
            {
                var adlerChecksum = AdlerChecksum.Checksum(Buffer, HeaderLength, Length);
                AddChecksumToHeader(BitConverter.GetBytes(adlerChecksum));
            }
            AddLengthToHeader();

        }
        private byte[] GetLengthBytes() => BitConverter.GetBytes((ushort)Length);

        private void AddLengthToHeader()
        {
            var length = GetLengthBytes();

            for (int i = 0; i < length.Length; i++)
            {
                WriteByte(length[i], i);
            }
        }

        private void AddChecksumToHeader(byte[] checksum)
        {
            var cIndex = 0;
            for (int i = 2; i < HeaderLength; i++)
            {
                WriteByte(checksum[cIndex++], i);
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
            Length++;
            Buffer[position] = b;
        }
        private void WriteByte(byte b)
        {
            Length++;
            Buffer[_position++] = b;
        }
    }
}