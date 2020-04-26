using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Server.Contracts.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Packets.Incoming
{
    public class UseItemPacket : IncomingPacket
    {
        public Location Location{ get; }
        public ushort ClientId{ get; }
        public byte StackPosition { get; set; }
        public byte Index { get; set; }

        public UseItemPacket(IReadOnlyNetworkMessage message)
        {
            Location = message.GetLocation();
            ClientId = message.GetUInt16();
            StackPosition = message.GetByte();
            Index = message.GetByte();
        }
    }
}
