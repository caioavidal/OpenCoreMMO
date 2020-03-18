using System;
using System.Collections.Generic;
using System.Text;
using NeoServer.Networking.Packets.Security;
using NeoServer.Server.Model;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class CharacterListPacket : OutgoingPacket
    {
        public CharacterListPacket(Account account)
        {
            AddCharList(account);
        }

        private void AddCharList(Account account)
        {
            OutputMessage.AddPayloadLengthSpace();

            //output.AddByte(0x14); todo: modt
            OutputMessage.AddByte(0x64); //todo charlist
            OutputMessage.AddByte((byte)account.Players.Count);
            foreach (var player in account.Players)
            {
                OutputMessage.AddString(player.Name);
                OutputMessage.AddString("NeoServer"); //todo change to const
                OutputMessage.AddByte(127);
                OutputMessage.AddByte(0);
                OutputMessage.AddByte(0);
                OutputMessage.AddByte(1);
                OutputMessage.AddUInt16(7172);
            }
            OutputMessage.AddUInt16((ushort)account.PremiumTime);

            OutputMessage.AddPayloadLength();
        }

        private void AddWorlds()
        {
            OutputMessage.AddByte(1); // number of worlds
            OutputMessage.AddByte(0); // world id
            OutputMessage.AddString("NeoServer"); //todo change to const
            OutputMessage.AddString("localhost");
            OutputMessage.AddUInt16(7171);
            OutputMessage.AddByte(0);
        }
    }
}
