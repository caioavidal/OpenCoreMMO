using NeoServer.Networking;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Repositories;
using NeoServer.Server.Model;
using System;
using System.Linq;

namespace NeoServer.Server.Handlers.Authentication
{
    public class PlayerLogInEventHandler : PacketHandler
    {
        private readonly IAccountRepository _repository;
        private readonly ServerState _serverState;

        public PlayerLogInEventHandler(IAccountRepository repository, ServerState serverState)
        {
            _repository = repository;
            _serverState = serverState;
        }

        public async override void HandlerMessage(IReadOnlyNetworkMessage message, Connection connection)
        {
            var packet = new PlayerLogInPacket(message);

            connection.SetXtea(packet.Xtea);

            if (_serverState.Status == ServerStatus.Closed)
            {
                connection.Send(new GameServerDisconnectPacket("Server is currently closed.\nPlease try again later."));
            }


        }
    }
}
