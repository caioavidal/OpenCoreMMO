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
            var charListOutput = GetCharList(account);
            var payloadLength = charListOutput.BufferLength;

            OutputMessage.AddUInt16((ushort)payloadLength);
            //todo: refact
            for (int i = 0; i < payloadLength; i++)
            {
                OutputMessage.AddByte(charListOutput.Buffer[i]);
            }
        }
        private NetworkMessage GetCharList(Account account)
        {

            var output = new NetworkMessage();
            //output.AddByte(0x14); todo: modt
            output.AddByte(0x64); //todo charlist
            output.AddByte((byte)account.Players.Count);
            foreach (var player in account.Players)
            {
                output.AddString(player.Name);
                output.AddString("NeoServer"); //todo change to const
                output.AddByte(127);
                output.AddByte(0);
                output.AddByte(0);
                output.AddByte(1);
                output.AddUInt16(7172);
            }
            output.AddUInt16((ushort)account.PremiumTime);
            return output;
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
