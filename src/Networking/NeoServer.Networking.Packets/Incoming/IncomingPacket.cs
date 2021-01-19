using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Incoming
{
    public abstract class IncomingPacket
    {
        public uint[] Xtea { get; } = new uint[4];

        protected void LoadXtea(IReadOnlyNetworkMessage message)
        {
            for (int i = 0; i < 4; i++)
            {
                Xtea[i] = message.GetUInt32();
            }
        }
    }
}
