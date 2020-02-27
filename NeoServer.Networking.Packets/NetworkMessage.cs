namespace NeoServer.Networking.Packets
{
    using System;
    using System.Text;

    public class NetworkMessage
    {
        public byte[] Buffer { get; private set; }
        public int BytesRead
        {
            get
            {
                return _position;
            }
        }
        public int Length { get; private set; } = 0;

        private int _position;

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
        }

        public ushort GetUInt16()
        {
            var to = _position + 2;
            var value = BitConverter.ToUInt16(Buffer[_position..to], 0);
            _position = to;
            return value;

        }
        public uint GetUInt32()
        {
            var to = _position + 4;
            var value = BitConverter.ToUInt32(Buffer[_position..to], 0);
            _position = to;
            return value;
        }

        public void SkipBytes(int length) => _position += length;

        public void ResetPosition() => _position = 0;

        public byte GetByte()
        {
            var to = _position + 1;
            var value = Buffer[_position..to];
            _position = to;
            return value[0];
        }
        public byte[] GetBytes(int length)
        {
            var to = _position + length;
            var value = Buffer[_position..to];
            _position = to;
            return value;
        }



        public string GetString()
        {

            var length = GetUInt16();
            var to = _position + length;

            var value = Encoding.UTF8.GetString(Buffer[_position..to]);
            _position = to;

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

        // public void AddHeader(bool addChecksum)
        // {
        //     if (addChecksum)
        //     {
        //         var adlerChecksum = AdlerChecksum.Checksum(Buffer, HeaderLength, Length);
        //         AddChecksumToHeader(BitConverter.GetBytes(adlerChecksum));
        //     }
        //     AddLengthToHeader();

        // }
        private byte[] GetLengthBytes() => BitConverter.GetBytes((ushort)Length);

        private void AddLengthToHeader()
        {
            var length = GetLengthBytes();

            for (int i = 0; i < length.Length; i++)
            {
                WriteByte(length[i], i);
            }
        }

        // private void AddChecksumToHeader(byte[] checksum)
        // {
        //     var cIndex = 0;
        //     for (int i = 2; i < HeaderLength; i++)
        //     {
        //         WriteByte(checksum[cIndex++], i);
        //     }
        // }

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
