using NeoServer.Networking.Packets.Messages;
using NeoServer.Server.Security;

namespace NeoServer.Networking.Packets.Incoming
{
    public class ChangeModePacket : IncomingPacket
    {
        public string Account { get; }
        public string Password { get; }
        public ushort ProtocolVersion { get; }

        public ChangeModePacket(IReadOnlyNetworkMessage message)
        {
            
        }

        public bool IsValid() => !(string.IsNullOrWhiteSpace(Account) || string.IsNullOrWhiteSpace(Password));


    }
}
