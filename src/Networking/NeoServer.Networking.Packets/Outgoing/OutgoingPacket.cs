using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing
{
    public abstract class OutgoingPacket : IOutgoingPacket
    {
        public virtual bool Disconnect { get; protected set; } = false;
        public abstract void WriteToMessage(INetworkMessage message);
    }

}
