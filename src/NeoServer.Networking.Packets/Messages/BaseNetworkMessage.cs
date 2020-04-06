//namespace NeoServer.Networking.Packets
//{
//    using NeoServer.Networking.Packets.Messages;
//    using NeoServer.Server.Contracts.Network;
//    using NeoServer.Server.Security;
//    using System;
//    using System.Linq;
//    using System.Text;

//    public class BaseNetworkMessage: INetworkMessage
//    {
//        protected byte[] Buffer { get; private set; }
//        public int Length { get; protected set; } = 0;

//        public byte[] GetMessageInBytes() => Length == 0 ? Buffer.ToArray() : Buffer[0..Length].ToArray();

    

//        public BaseNetworkMessage(byte[] buffer)
//        {
//            Buffer = buffer;
//        }
//    }
//}
