using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Networking.Packets.Outgoing.Login;
using NeoServer.Networking.Packets.Security;
using NeoServer.Server.Common.Contracts.Network;
using Serilog.Core;

namespace NeoServer.Networking.Packets.Connection
{
    public class Connection : IConnection
    {
        private const int NETWORK_MESSAGE_MAXSIZE = 24590 - 16;
        private const int HEADER_LENGTH = 2;
        private readonly object connectionLock;
        private readonly Logger logger;

        private readonly Socket Socket;
        private readonly Stream Stream;
        private readonly object writeLock;

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

        public int BeginStreamReadCalls { get; set; }

        public bool Closed => !Stream.CanRead || !Socket.Connected;

        public event EventHandler<IConnectionEventArgs> OnProcessEvent;
        public event EventHandler<IConnectionEventArgs> OnCloseEvent;
        public event EventHandler<IConnectionEventArgs> OnPostProcessEvent;

        public Queue<IOutgoingPacket> OutgoingPackets { get; private set; }
        public IReadOnlyNetworkMessage InMessage { get; }

        public uint[] XteaKey { get; private set; }
        public uint CreatureId { get; private set; }
        public bool IsAuthenticated { get; private set; }

        public bool Disconnected { get; private set; }
        public long LastPingRequest { get; set; }
        public long LastPingResponse { get; set; }

        public string IP { get; }

        public void BeginStreamRead()
        {
            if (!Stream.CanRead || !Socket.Connected || Disconnected) return;

            try
            {
                Stream.BeginRead(InMessage.Buffer, 0, HEADER_LENGTH, OnRead, null);
            }
            catch
            {
                logger.Error("Error on stream read");
            }
        }

        public void SetXtea(uint[] xtea)
        {
            XteaKey = xtea;
        }

        public void Close(bool force = false)
        {
            //todo needs to remove this connection from pool
            lock (connectionLock)
            {
                if (!Socket.Connected)
                {
                    if (Stream.CanRead) Stream.Close();
                    return;
                }

                if (OutgoingPackets == null || !OutgoingPackets.Any() || force)
                {
                    CloseSocket();
                }
            }

            // Tells the subscribers of this event that this connection has been closed.
            OnCloseEvent?.Invoke(this, new ConnectionEventArgs(this));
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

            var encryptedMessage = Xtea.Encrypt(message, XteaKey);

            SendMessage(encryptedMessage);
        }

        /// <summary>
        ///     Sends all packets in connection's outgoing packets queue and clean
        /// </summary>
        public void Send()
        {
            if (!OutgoingPackets.Any()) return;

            var message = new NetworkMessage();

            while (OutgoingPackets.TryDequeue(out var packet))
            {
                logger.Debug("To {PlayerId}: {name}", CreatureId, packet.GetType().Name);
                packet.WriteToMessage(message);
            }

            message.AddLength();

            var encryptedMessage = Xtea.Encrypt(message, XteaKey);
            SendMessage(encryptedMessage);
        }

        public void Disconnect(string text)
        {
            var message = new NetworkMessage();

            new LoginFailurePacket(text).WriteToMessage(message);

            message.AddLength();

            var encryptedMessage = Xtea.Encrypt(message, XteaKey);

            SendMessage(encryptedMessage);
            Close();
        }

        public void SetAsAuthenticated()
        {
            IsAuthenticated = true;
        }

        public void SetConnectionOwner(IPlayer player)
        {
            if (CreatureId != 0) throw new InvalidOperationException("Connection already has a Player Id");
            SetAsAuthenticated();

            OutgoingPackets = new Queue<IOutgoingPacket>();

            CreatureId = player.CreatureId;
        }

        private void OnRead(IAsyncResult ar)
        {
            var clientDisconnected = !CompleteRead(ar);
            if (clientDisconnected && !IsAuthenticated)
            {
                Close();
                return;
            }

            if (clientDisconnected && IsAuthenticated) Disconnected = true;

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
                    if (Socket.Connected == false)
                    {
                        Close();
                        return false;
                    }

                    if (Socket.Available == 0) return false;

                    var read = Stream.EndRead(ar);

                    var size = BitConverter.ToUInt16(InMessage.Buffer, 0) + 2;

                    if (size >= NETWORK_MESSAGE_MAXSIZE)
                    {
                        Close(true);
                        return false;
                    }

                    while (read < size)
                        if (Stream.CanRead)
                            read += Stream.Read(InMessage.Buffer, read, size - read);

                    InMessage.Resize(size);
                }

                return true;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                Close();
            }

            return false;
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
                    if (Closed || !Socket.Connected || Disconnected) return;
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
    }
}