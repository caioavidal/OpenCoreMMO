using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class SelfAppearPacket : OutgoingPacket
    {
        private byte GraphicsSpeed => 0x32; //  beat duration (50)
        private byte CanReportBugs => 0x00;
        private readonly IPlayer player;

        public SelfAppearPacket(IPlayer player)
        {
            this.player = player;
        }

        public override void WriteToMessage(INetworkMessage message)
        {

            message.AddByte((byte)GameOutgoingPacketType.SelfAppear);

            message.AddUInt32(player.CreatureId);
            message.AddUInt16(GraphicsSpeed);
            message.AddByte(CanReportBugs); 	// can report bugs? todo: create tutor account type
        }
    }
}
