using NeoServer.Game.Commands;
using NeoServer.Game.Contracts;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Contracts.Repositories;
using System.Linq;

namespace NeoServer.Server.Handlers.Authentication
{
    public class PlayerLogInHandler : PacketHandler
    {
        private readonly IAccountRepository repository;
        private readonly ServerState serverState;

        private readonly Game game;
        private readonly IMap map;

        private readonly IDispatcher dispatcher;

        public PlayerLogInHandler(IAccountRepository repository, ServerState serverState,
         Game game, IMap map, IDispatcher dispatcher)
        {
            this.repository = repository;
            this.serverState = serverState;
            this.game = game;
            this.map = map;
            this.dispatcher = dispatcher;
        }

        public async override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var packet = new PlayerLogInPacket(message);

            connection.SetXtea(packet.Xtea);

            var accountRecord = await repository.FirstOrDefaultAsync(a => a.AccountName == packet.Account && a.Password == packet.Password);
            var playerRecord = accountRecord.Players.FirstOrDefault(p => p.CharacterName == packet.CharacterName);

            if (serverState.Status == ServerStatus.Closed)
            {
                connection.Send(new GameServerDisconnectPacket("Server is currently closed.\nPlease try again later."));
                return;
            }

            dispatcher?.Dispatch(new PlayerLogInCommand(playerRecord, connection));
            //todo: add more validations

        }
    }
}
