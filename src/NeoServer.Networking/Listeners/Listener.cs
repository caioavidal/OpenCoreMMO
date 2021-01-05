using NeoServer.Networking.Protocols;
using NeoServer.Server.Contracts.Network;
using Serilog.Core;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NeoServer.Networking.Listeners
{
    public abstract class Listener : TcpListener, IListener
    {
        private readonly IProtocol protocol;
        private readonly Logger logger;
        public Listener(int port, IProtocol protocol, Logger logger) : base(IPAddress.Any, port)
        {
            this.protocol = protocol;
            this.logger = logger;
        }

        public void BeginListening()
        {
            Task.Run(async () =>
            {
                Start();
                logger.Information("{protocol} is online", protocol);

                while (true)
                {
                    var connection = await CreateConnection();

                    protocol.OnAccept(connection);
                }

            });
        }

        private async Task<IConnection> CreateConnection()
        {
            var socket = await AcceptSocketAsync().ConfigureAwait(false);

            var connection = new Connection(socket);

            connection.OnCloseEvent += OnConnectionClose;
            connection.OnProcessEvent += protocol.ProcessMessage;
            connection.OnPostProcessEvent += protocol.PostProcessMessage;
            return connection;
        }

        private void OnConnectionClose(object sender, IConnectionEventArgs args)
        {
            // De-subscribe to this event first.
            args.Connection.OnCloseEvent -= OnConnectionClose;
            args.Connection.OnProcessEvent -= protocol.ProcessMessage;
            args.Connection.OnPostProcessEvent -= protocol.PostProcessMessage;
        }

        public void EndListening()
        {
            Stop();
        }
    }
}
