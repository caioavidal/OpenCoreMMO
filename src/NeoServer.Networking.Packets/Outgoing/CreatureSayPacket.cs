using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class CreatureSayPacket : OutgoingPacket
    {
        private readonly ICreature creature;
        private readonly SpeechType talkType;
        private readonly string textMessage;

        public CreatureSayPacket(ICreature creature, SpeechType talkType, string textMessage)
        {
            this.creature = creature;
            this.talkType = talkType;
            this.textMessage = textMessage;
        }

        //todo: this code is duplicated?
        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte(0xAA);
            message.AddUInt32(0x00);

            message.AddString(creature.Name);

            //Add level only for players
            if (creature is IPlayer player)
            {
                message.AddUInt16(player.Level);
            }
            else
            {
                message.AddUInt16(0x00);
            }

            message.AddByte((byte)talkType);

            message.AddLocation(creature.Location);

            message.AddString(textMessage);
        }

    }
}
