namespace NeoServer.Networking.Packets
{
    using NeoServer.Game.Contracts.Items;
    using NeoServer.Game.Enums.Location.Structs;
    using NeoServer.Networking.Packets.Messages;
    using NeoServer.Server.Contracts.Network;
    using NeoServer.Server.Security;
    using System;
    using System.Text;

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

        public void AddLocation(Location location)
        {
            AddUInt16((ushort)location.X);
            AddUInt16((ushort)location.Y);
            AddByte((byte)location.Z);
        }

        public void AddString(string value)
        {
            AddUInt16((ushort)value.Length);
            WriteBytes(Encoding.UTF8.GetBytes(value));
        }

        public void AddItem(IItem item)
        {

            if (item == null)
            {
                //todo log
                return;
            }

            AddUInt16(item.Type.ClientId);

            if (item.IsCumulative)
            {
                AddByte(item.Amount);
            }
            else if (item.IsLiquidPool || item.IsLiquidSource || item.IsLiquidContainer)
            {
                AddByte((byte)item.LiquidType);
            }
        }

        public void AddUInt32(uint value) => WriteBytes(BitConverter.GetBytes(value));
        public void AddUInt16(ushort value) => WriteBytes(BitConverter.GetBytes(value));

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

        public void AddPayloadLength()
        {
            var bytes = BitConverter.GetBytes((ushort)Length);
            Buffer[0] = bytes[0];
            Buffer[1] = bytes[1];
        }

        public void AddPayloadLengthSpace() => Cursor += 2;

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
