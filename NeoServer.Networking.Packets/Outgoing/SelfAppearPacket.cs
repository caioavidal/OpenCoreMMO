using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class SelfAppearPacket : OutgoingPacket
    {
        private byte GraphicsSpeed => 0x32; //  beat duration (50)
        private byte CanReportBugs => 0x00;

        public SelfAppearPacket(IPlayer player): base(false)
        {
            OutputMessage.AddByte((byte)GameOutgoingPacketType.SelfAppear);

            OutputMessage.AddUInt32(player.CreatureId);
            OutputMessage.AddUInt16(GraphicsSpeed);
            OutputMessage.AddByte(CanReportBugs); 	// can report bugs? todo: create tutor account type
        }
    }
}
