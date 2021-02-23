using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class PlayerSendPrivateMessagePacket : OutgoingPacket
    {
        public PlayerSendPrivateMessagePacket(ISociableCreature from, SpeechType talkType, string message)
        {
            From = from;
            TalkType = talkType;
            Message = message;
        }

        public ISociableCreature From { get;  }
        public SpeechType TalkType { get; }
        public string Message { get; }
        public override void WriteToMessage(INetworkMessage message)
        {
			if (TalkType == SpeechType.None) return;

		
			message.AddByte((byte)GameOutgoingPacketType.SendPrivateMessage);
			uint statementId = 0;

			message.AddUInt32(++statementId);

			if (From is not null)
			{
				message.AddString(From.Name);
				if(From is IPlayer player) message.AddUInt16(player.Level);	
				else message.AddUInt16(0x00);
			}
			else
			{
				message.AddUInt16(0x00);
			}
			message.AddByte((byte)TalkType);
			message.AddString(Message);
		}
    }
}
