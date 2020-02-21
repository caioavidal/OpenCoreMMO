using NeoServer.Server.Model;

namespace NeoServer.Networking.Packets.Incoming
{
    public class AccountLoginPacket : IncomingPacket
    {
        public AccountLoginPacket(NetworkMessage message) : base(message)
        {
            //var packetPayload = message.GetUInt16();
            //var tcpPayload = packetPayload + 2;
            //message.SkipBytes(7);
            ////var os = message.GetUInt16();
            //Version = message.GetUInt16();

            ////var files = message.GetBytes(12);

            //// todo: version validation

            //message.SkipBytes(10);

            //var encryptedData = message.GetBytes(tcpPayload - message.BytesRead);

            Version = 860;
            Model = new Account("caio", "123");



            //var decryptedData = new InputMessage(RSA.Decrypt(encryptedData));


            //LoadXtea(decryptedData);
            //LoadAccount(decryptedData);
        }

        public int Version { get; }

        public override IServerModel Model { get; }
    }
}
