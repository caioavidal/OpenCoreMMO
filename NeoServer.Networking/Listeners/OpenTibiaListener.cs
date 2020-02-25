using NeoServer.Networking.Protocols;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NeoServer.Networking.Listeners
{
    public abstract class OpenTibiaListener : TcpListener, IOpenTibiaListener
    {
        private readonly IProtocol _protocol;
        public OpenTibiaListener(int port, IProtocol protocol) : base(IPAddress.Any, port)
        {
            _protocol = protocol;
        }

        public void BeginListening()
        {
            Start();
            BeginAcceptSocket(OnAccept, this);
        }

        public void OnAccept(IAsyncResult ar)
        {
            var connection = CreateConnection();

            //  _connections.Add(connection);

            _protocol.OnAcceptNewConnection(connection, ar);

            BeginAcceptSocket(OnAccept, this);
        }

        private Connection CreateConnection()
        {
            var connection = new Connection();

            connection.OnCloseEvent += OnConnectionClose;
            connection.OnProcessEvent += _protocol.ProcessMessage;
            connection.OnPostProcessEvent += _protocol.PostProcessMessage;
            return connection;
        }

        private void OnConnectionClose(object sender, ConnectionEventArgs args)
        {
            // De-subscribe to this event first.
            args.Connection.OnCloseEvent -= OnConnectionClose;
            args.Connection.OnProcessEvent -= _protocol.ProcessMessage;
            args.Connection.OnPostProcessEvent -= _protocol.PostProcessMessage;

          //  _connections.Remove(connection);
        }


        public void EndListening()
        {
            Stop();
        }
    }
}
