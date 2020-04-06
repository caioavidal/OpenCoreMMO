using NeoServer.Networking.Protocols;
using NeoServer.Server.Contracts.Network;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
            Task.Run(async () =>
            {
                Start();
                Console.WriteLine($"{_protocol} is online");

                while (true)
                {
                    var connection = await CreateConnection();

                    _protocol.OnAcceptNewConnection(connection);
                }
                
            });
        }

        private async Task<IConnection> CreateConnection()
        {
            var socket = await AcceptSocketAsync().ConfigureAwait(false);

            var connection = new Connection(socket);

            connection.OnCloseEvent += OnConnectionClose;
            connection.OnProcessEvent += _protocol.ProcessMessage;
            connection.OnPostProcessEvent += _protocol.PostProcessMessage;
            return connection;
        }

        private void OnConnectionClose(object sender, IConnectionEventArgs args)
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
