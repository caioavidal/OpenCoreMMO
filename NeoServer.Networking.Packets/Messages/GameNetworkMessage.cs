    using System;
    using System.Linq;
    using NeoServer.Networking.Packets;
    using NeoServer.Networking.Packets.Messages;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Security;

    public class GameNetworkMessage:INetworkMessage
    {
        public ReadOnlyNetworkMessage Header { get; private set; }
        private NetworkMessage Body { get; set; }

        public GameNetworkMessage(NetworkMessage body)
        {
            Body = body;
            AddHeader(true);
        }


        private void AddHeader(bool addChecksum)
        {
            var checkSumBytes = new byte[4];
            if (addChecksum)
            {
                var adlerChecksum = AdlerChecksum.Checksum(Body.GetMessageInBytes(), 0, Body.Length); //todo: 6 is the header length
                checkSumBytes = BitConverter.GetBytes(adlerChecksum);
            }
            var lengthInBytes = BitConverter.GetBytes((ushort)(Body.Length + checkSumBytes.Length));

            Header = new ReadOnlyNetworkMessage(lengthInBytes.Concat(checkSumBytes).ToArray());
        }


        /// <summary>
        /// Get network message with the body buffer within header(length and adler)
        /// </summary>
        /// <returns></returns>
        public byte[] GetMessageInBytes()
        {
                return Header.GetMessageInBytes().Concat(Body.GetMessageInBytes()).ToArray();
        }
    }