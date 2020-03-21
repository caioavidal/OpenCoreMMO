using NeoServer.Networking.Packets;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace NeoServer.Networking
{

    public class Connection
    {
        public event EventHandler<ConnectionEventArgs> OnProcessEvent;
        public event EventHandler<ConnectionEventArgs> OnCloseEvent;
        public event EventHandler<ConnectionEventArgs> OnPostProcessEvent;

        private Socket Socket;
        private Stream Stream;

        private byte[] Buffer = new byte[16394];

        public IReadOnlyNetworkMessage InMessage { get; private set; }

        public uint[] XteaKey { get; private set; }
        public uint PlayerId { get; set; }
        public bool IsAuthenticated { get; set; } = false;

        public void OnAccept(IAsyncResult ar)
        {
            if (ar == null)
            {
                throw new ArgumentNullException(nameof(ar));
            }

            Socket = ((TcpListener)ar.AsyncState).EndAcceptSocket(ar);
            Stream = new NetworkStream(Socket);

            BeginStreamRead();

        }

        public Connection()
        {
            Socket = null;
            Stream = null;
            XteaKey = new uint[4];
            ResetBuffer();
        }
        public void BeginStreamRead() => Stream.BeginRead(Buffer, 0, 16394, OnRead, null);

        public void SetXtea(uint[] xtea)
        {
            XteaKey = xtea;
        }

        public void ResetBuffer()
        {
            Buffer = new byte[16394];
        }

        private void OnRead(IAsyncResult ar)
        {
            //if (!CompleteRead(ar))
            //{
            //    return;
            //}
            if (!Stream.CanRead)
            {
                return;
            }

            int length = Stream.EndRead(ar);

            if (length == 0)
            {
                Close();
                return;
            }



            try
            {
                //Stream.Read(Buffer, 0, length);

                InMessage = new ReadOnlyNetworkMessage(Buffer, length);
                var eventArgs = new ConnectionEventArgs(this);
                OnProcessEvent(this, eventArgs);

            }
            catch (Exception e)
            {
                // Invalid data from the client
                // TODO: I must not swallow exceptions.
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);

                // TODO: is closing the connection really necesary?
                // Close();
            }
        }



        public void Close()
        {
            Stream.Close();
            Socket.Close();

            // Tells the subscribers of this event that this connection has been closed.
            OnCloseEvent?.Invoke(this, new ConnectionEventArgs(this));

        }

        private void SendMessage(INetworkMessage message, bool disconnect = false)
        {
            try
            {


                var streamMessage = message.AddHeader();
                Stream.BeginWrite(streamMessage, 0, streamMessage.Length, null, null);

                var eventArgs = new ConnectionEventArgs(this);
                OnPostProcessEvent?.Invoke(this, eventArgs);

            }
            catch (ObjectDisposedException)
            {
                Close();
            }
        }

        public void Send(IOutgoingPacket packet, bool encrypt = true)
        {

            var message = encrypt ? Packets.Security.Xtea.Encrypt(packet.GetMessage(), XteaKey) : packet.GetMessage();


            SendMessage(message, packet.Disconnect);

        }

        public void Send(Queue<OutgoingPacket> outgoingPackets)
        {

            var joinedPackets = new byte[16394];
            int totalLength = 0;

            foreach (var outPacket in outgoingPackets)
            {
                var networkMessage = outPacket.GetMessage();
                var buffer = networkMessage.Buffer;

                System.Buffer.BlockCopy(buffer, 0, joinedPackets, totalLength, networkMessage.Length);

                totalLength += networkMessage.Length;
            }

            INetworkMessage joinedMessage = new NetworkMessage(joinedPackets, totalLength);
            joinedMessage.AddLength();

            joinedMessage = Packets.Security.Xtea.Encrypt(joinedMessage, XteaKey);

            SendMessage(joinedMessage);
        }
        public void Send(string text)
        {
            var message = Packets.Security.Xtea.Encrypt(new TextMessagePacket(text).GetMessage(), XteaKey);
            SendMessage(message);
        }
    }
}
