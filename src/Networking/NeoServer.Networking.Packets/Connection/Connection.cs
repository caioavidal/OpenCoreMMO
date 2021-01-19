using NeoServer.Networking.Packets;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;

namespace NeoServer.Networking
{
    public class Connection : IConnection
    {
        private const int NETWORK_MESSAGE_MAXSIZE = 24590 - 16;
        private const int HEADER_LENGTH = 2;

        public event EventHandler<IConnectionEventArgs> OnProcessEvent;
        public event EventHandler<IConnectionEventArgs> OnCloseEvent;
        public event EventHandler<IConnectionEventArgs> OnPostProcessEvent;

        private Socket Socket;
        private Stream Stream;
        private object writeLock;
        private object connectionLock;

        public Queue<IOutgoingPacket> OutgoingPackets { get; private set; }
        public IReadOnlyNetworkMessage InMessage { get; private set; }

        public uint[] XteaKey { get; private set; }
        public uint PlayerId { get; private set; }
        public bool IsAuthenticated { get; private set; } = false;

        public bool Disconnected { get; private set; } = false;

        public int BeginStreamReadCalls { get; set; }
        public long LastPingRequest { get; set; }
        public long LastPingResponse { get; set; }

        public string IP { get; }

        public bool Closed => !Stream.CanRead || !Socket.Connected;
        private Logger logger;

        public Connection(Socket socket, Logger logger)
        {
            Socket = socket;
            IP = socket.RemoteEndPoint.ToString();
            Stream = new NetworkStream(Socket);
            XteaKey = new uint[4];
            IsAuthenticated = false;
            InMessage = new ReadOnlyNetworkMessage(new byte[16394], 0);
            writeLock = new object();
            connectionLock = new object();
            this.logger = logger;
        }
        public void BeginStreamRead()
        {
            if (!Stream.CanRead || !Socket.Connected || Disconnected) return;

            try
            {
                Stream.BeginRead(InMessage.Buffer, 0, HEADER_LENGTH, OnRead, null);
            }
            catch
            {
                Console.WriteLine("Error on stream read");
            }
        }

        public void SetXtea(uint[] xtea)
        {
            XteaKey = xtea;
        }
        private void OnRead(IAsyncResult ar)
        {

            var clientDisconnected = !this.CompleteRead(ar);
            if (clientDisconnected && !IsAuthenticated)
            {
                Close();
                return;
            }
            if (clientDisconnected && IsAuthenticated)
            {

                Disconnected = true;
            }

            var eventArgs = new ConnectionEventArgs(this);

            try
            {
                OnProcessEvent?.Invoke(this, eventArgs);
                BeginStreamRead();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);

                // TODO: is closing the connection really necesary?
                // Disconnected = true;
                // OnProcessEvent?.Invoke(this, eventArgs);
            }
        }

        private bool CompleteRead(IAsyncResult ar)
        {
            try
            {
                lock (connectionLock)
                {

                    if (Socket.Available == 0)
                    {                      
                        return false;
                    }

                    int read = Stream.EndRead(ar);

                    int size = BitConverter.ToUInt16(InMessage.Buffer, 0) + 2;

                    if (size >= NETWORK_MESSAGE_MAXSIZE)
                    {
                        Close(true);
                        return false;
                    }

                    while (read < size)
                    {
                        if (Stream.CanRead)
                        {
                            read += Stream.Read(InMessage.Buffer, read, size - read);
                        }
                    }

                    InMessage.Resize(size);
                }

                return true;
            }
            catch (Exception e)
            {
                Close();
            }

            return false;
        }

        public void Close(bool force = false)
        {
            //todo needs to remove this connection from pool
            lock (connectionLock)
            {
                if (!Socket.Connected)
                {
                    if (Stream.CanRead)
                    {
                        Stream.Close();
                    }
                    return;
                }

                if (OutgoingPackets == null || !OutgoingPackets.Any() || force)
                {
                    CloseSocket();
                }
                else
                {
                    //need to close connection when outgoing queue is empty;
                }
            }

            // Tells the subscribers of this event that this connection has been closed.
            OnCloseEvent?.Invoke(this, new ConnectionEventArgs(this));

        }

        private void CloseSocket()
        {
            try
            {
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Close();
            }
            catch (Exception)
            {

                Console.WriteLine("Error on socket closing");
            }

        }

        private void SendMessage(INetworkMessage message)
        {
            try
            {
                lock (writeLock)
                {
                    if (Closed || !Socket.Connected || Disconnected)
                    {
                        return;
                    }
                    var streamMessage = message.AddHeader();

                    Stream.BeginWrite(streamMessage, 0, streamMessage.Length, null, null);

                }

                var eventArgs = new ConnectionEventArgs(this);
                OnPostProcessEvent?.Invoke(this, eventArgs);
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("Network error - Send Message fail");
                Close();
            }
        }

        public void SendFirstConnection()
        {
            var message = new NetworkMessage();

            new FirstConnectionPacket().WriteToMessage(message);

            SendMessage(message);
        }

        public void Send(IOutgoingPacket packet)
        {
            var message = new NetworkMessage();

            packet.WriteToMessage(message);

            message.AddLength();

            var encryptedMessage = Packets.Security.Xtea.Encrypt(message, XteaKey);

            SendMessage(encryptedMessage);

        }

        /// <summary>
        /// Sends all packets in connection's outgoing packets queue and clean
        /// </summary>
        public void Send()
        {
            if (!OutgoingPackets.Any()) return;

            var message = new NetworkMessage();

            while (OutgoingPackets.TryDequeue(out var packet))
            {
                logger.Debug("To {PlayerId}: {name}", PlayerId, packet.GetType().Name);
                packet.WriteToMessage(message);
            }

            message.AddLength();

            var encryptedMessage = Packets.Security.Xtea.Encrypt(message, XteaKey);
            SendMessage(encryptedMessage);
        }

        public void Disconnect(string text)
        {
            var message = new NetworkMessage();

            new LoginFailurePacket(text).WriteToMessage(message);

            message.AddLength();

            var encryptedMessage = Packets.Security.Xtea.Encrypt(message, XteaKey);

            SendMessage(encryptedMessage);
            Close();
        }

        public void SetAsAuthenticated() => IsAuthenticated = true;

        public void SetConnectionOwner(IPlayer player)
        {
            if (PlayerId != 0)
            {
                throw new InvalidOperationException("Connection already has a Player Id");
            }
            SetAsAuthenticated();

            OutgoingPackets = new Queue<IOutgoingPacket>();

            PlayerId = player.CreatureId;
        }
    }
}
