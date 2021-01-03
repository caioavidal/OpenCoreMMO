using NeoServer.Data.Model;
using NeoServer.Server.Contracts.Network;
using System.Linq;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class CharacterListPacket : OutgoingPacket
    {
        private readonly AccountModel _acountModel;
        public CharacterListPacket(AccountModel account)
        {
            _acountModel = account;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            AddCharList(message);
        }

        private void AddCharList(INetworkMessage message)
        {
            message.AddByte(0x64); //todo charlist
            message.AddByte((byte)_acountModel.Players.Count());
            foreach (var player in _acountModel.Players)
            {
                message.AddString(player.Name);
                message.AddString("NeoServer"); //todo change to const
                message.AddByte(127);
                message.AddByte(0);
                message.AddByte(0);
                message.AddByte(1);
                message.AddUInt16(7172);
            }
            message.AddUInt16((ushort)_acountModel.PremiumTime);
        }

        // private void AddWorlds()
        // {
        //     message.AddByte(1); // number of worlds
        //     message.AddByte(0); // world id
        //     message.AddString("NeoServer"); //todo change to const
        //     message.AddString("localhost");
        //     message.AddUInt16(7171);
        //     message.AddByte(0);
        // }
    }
}
