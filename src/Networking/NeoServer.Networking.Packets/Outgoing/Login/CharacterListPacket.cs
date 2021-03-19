using NeoServer.Data.Model;
using NeoServer.Server.Contracts.Network;
using System.Linq;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class CharacterListPacket : OutgoingPacket
    {
        private readonly AccountModel _acountModel;
        private readonly string serverName;
        public CharacterListPacket(AccountModel account, string serverName)
        {
            _acountModel = account;
            this.serverName = serverName;
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
                message.AddString(serverName ?? string.Empty);
                message.AddByte(127);
                message.AddByte(0);
                message.AddByte(0);
                message.AddByte(1);
                message.AddUInt16(7172);
            }
            message.AddUInt16((ushort)_acountModel.PremiumTime);
        }
    }
}
