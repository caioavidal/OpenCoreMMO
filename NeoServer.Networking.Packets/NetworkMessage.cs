namespace NeoServer.Networking.Packets
{
    using NeoServer.Server.Security;
    using System;
    using System.Linq;
    using System.Text;

    public class NetworkMessage
    {
        public byte[] Header { get; private set; }
        public int HeaderLength { get; private set; } = 6;
        public byte[] Buffer { get; private set; }
        public int BytesRead { get; private set; } = 0;
        public int Length { get; private set; } = 0;

        public GameIncomingPacketType IncomingPacketType
        {
            get
            {
                return (GameIncomingPacketType)BitConverter.ToInt16(Buffer[0..2]);
            }
        }

        public NetworkMessage(byte[] buffer)
        {
            Buffer = buffer;

            //var login = new LoginInput(loginData, handler);
        }
        public NetworkMessage()
        {
            Buffer = new byte[24590];
            Header = new byte[HeaderLength];
        }

        public ushort GetUInt16()
        {
            var to = BytesRead + 2;
            var value = BitConverter.ToUInt16(Buffer[BytesRead..to], 0);
            BytesRead = to;
            return value;

        }
        public uint GetUInt32()
        {
            var to = BytesRead + 4;
            var value = BitConverter.ToUInt32(Buffer[BytesRead..to], 0);
            BytesRead = to;
            return value;
        }

        public void SkipBytes(int length) => BytesRead += length;

        public void ResetPosition() => BytesRead = 0;

        public byte GetByte()
        {
            var to = BytesRead + 1;
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



        public string GetString()
        {

            var length = GetUInt16();
            var to = BytesRead + length;

            var value = Encoding.UTF8.GetString(Buffer[BytesRead..to]);
            BytesRead = to;

            return value;
        }

        ///// Write mode


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
                var adlerChecksum = AdlerChecksum.Checksum(Buffer, 6, Length); //todo: 6 is the header length
                checkSumBytes = BitConverter.GetBytes(adlerChecksum);
            }
            var lengthInBytes = BitConverter.GetBytes((ushort)Length + checkSumBytes.Length);

            Header = lengthInBytes.Concat(checkSumBytes).ToArray();
        }


        /// <summary>
        /// Get network message with the body buffer within header (length and adler)
        /// </summary>
        /// <returns></returns>
        public byte[] GetMessageInBytes()
        {
            AddHeader(true);
            return Header.Concat(Buffer).ToArray();
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
            Buffer[BytesRead++] = b;
        }


    }
}
