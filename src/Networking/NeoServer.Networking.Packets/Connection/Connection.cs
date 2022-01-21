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
using Serilog;

namespace NeoServer.Networking.Packets.Connection
{
    public class Connection : IConnection
    {
        private const int NETWORK_MESSAGE_MAXSIZE = 24590 - 16;
        private const int HEADER_LENGTH = 2;
        private readonly object _connectionLock;
        private readonly ILogger _logger;

        private readonly Socket _socket;
        private readonly Stream _stream;
        private readonly object _writeLock;

        public Connection(Socket socket, ILogger logger)
        {
            _socket = socket;
            IP = socket?.RemoteEndPoint?.ToString();
            _stream = new NetworkStream(_socket);
            XteaKey = new uint[4];
            IsAuthenticated = false;
            InMessage = new ReadOnlyNetworkMessage(new byte[16394], 0);
            _writeLock = new object();
            _connectionLock = new object();
            _logger = logger;
        }

        private bool Closed
        {
            get
            {
                lock (_connectionLock)
                {
                    return !_stream.CanRead || !_socket.Connected;
                }
            }
        }

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
            lock (_connectionLock)
            {
                if (!_stream.CanRead || !_socket.Connected || Disconnected) return;
            }

            try
            {
                lock (_connectionLock)
                {
                    _stream.BeginRead(InMessage.Buffer, 0, HEADER_LENGTH, OnRead, null);
                }
            }
            catch
            {
                _logger.Error("Error on stream read");
            }
        }

        public void SetXtea(uint[] xtea)
        {
            XteaKey = xtea;
        }

        public void Close(bool force = false)
        {
            //todo needs to remove this connection from pool
            lock (_connectionLock)
            {
                if (!_socket.Connected)
                {
                    if (_stream.CanRead) _stream.Close();
                    return;
                }

                if (OutgoingPackets == null || !OutgoingPackets.Any() || force) CloseSocket();
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
                _logger.Debug("To {PlayerId}: {name}", CreatureId, packet.GetType().Name);
                packet.WriteToMessage(message);
            }

            message.AddLength();

            var encryptedMessage = Xtea.Encrypt(message, XteaKey);
            SendMessage(encryptedMessage);
        }

        public void Disconnect()
        {
            Close();
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

                // TODO: is closing the connection really necessary?
                // Disconnected = true;
                // OnProcessEvent?.Invoke(this, eventArgs);
            }
        }

        private bool CompleteRead(IAsyncResult ar)
        {
            try
            {
                lock (_connectionLock)
                {
                    if (_socket.Connected == false)
                    {
                        Close();
                        return false;
                    }

                    if (_socket.Available == 0) return false;

                    var read = _stream.EndRead(ar);

                    var size = BitConverter.ToUInt16(InMessage.Buffer, 0) + 2;

                    if (size >= NETWORK_MESSAGE_MAXSIZE)
                    {
                        Close(true);
                        return false;
                    }

                    while (read < size)
                        if (_stream.CanRead)
                            read += _stream.Read(InMessage.Buffer, read, size - read);

                    InMessage.Resize(size);
                }

                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                Close();
            }

            return false;
        }

        private void CloseSocket()
        {
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
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
                lock (_writeLock)
                {
                    if (Closed || !_socket.Connected || Disconnected) return;
                    var streamMessage = message.AddHeader();

                    _stream.BeginWrite(streamMessage, 0, streamMessage.Length, null, null);
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