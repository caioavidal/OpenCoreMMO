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

        public bool Disconnected { get; private set; } = false;

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
            IsAuthenticated = false;
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

            if (!Stream.CanRead)
            {
                return;
            }

            int length = Stream.EndRead(ar);

            if (length == 0)
            {
                Disconnected = true;
            }

            try
            {

                InMessage = new ReadOnlyNetworkMessage(Buffer, length);
                //Buffer = new byte[16394];
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

        private void SendMessage(INetworkMessage message)
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

        public void SendFirstConnection()
        {
            var message = new NetworkMessage();

            new FirstConnectionPacket().WriteToMessage(message);

            SendMessage(message);
        }

        public void Send(IOutgoingPacket packet, bool encrypt = true)
        {
            var message = new NetworkMessage();

            packet.WriteToMessage(message);

            message.AddLength();

            var encryptedMessage = encrypt ? Packets.Security.Xtea.Encrypt(message, XteaKey) : message;

            SendMessage(encryptedMessage);

        }

        public void Send(Queue<OutgoingPacket> outgoingPackets)
        {
            var message = new NetworkMessage();

            foreach (var outPacket in outgoingPackets)
            {
                outPacket.WriteToMessage(message);

            }

            message.AddLength();

            var encryptedMessage = Packets.Security.Xtea.Encrypt(message, XteaKey);

            SendMessage(encryptedMessage);
        }
        public void Disconnect(string text)
        {
            var message = new NetworkMessage();

            new TextMessagePacket(text).WriteToMessage(message);

            message.AddLength();

            var encryptedMessage = Packets.Security.Xtea.Encrypt(message, XteaKey);

            SendMessage(encryptedMessage);
            Close();
        }
    }
}
