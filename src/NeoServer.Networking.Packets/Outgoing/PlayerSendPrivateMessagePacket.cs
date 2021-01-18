using NeoServer.Game.Common.Talks;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class PlayerSendPrivateMessagePacket : OutgoingPacket
    {
        public PlayerSendPrivateMessagePacket(IPlayer from, TalkType talkType, string message)
        {
            From = from;
            TalkType = talkType;
            Message = message;
        }

        public IPlayer From { get;  }
        public TalkType TalkType { get; }
        public string Message { get; }
        public override void WriteToMessage(INetworkMessage message)
        {
			if (TalkType == TalkType.None) return;

		
			message.AddByte((byte)GameOutgoingPacketType.SendPrivateMessage);
			uint statementId = 0;

			message.AddUInt32(++statementId);

			if (From is not null)
			{
				message.AddString(From.Name);
				message.AddUInt16(From.Level);	
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
