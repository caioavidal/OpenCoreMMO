using NeoServer.Data.Interfaces;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Commands;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Standalone;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Handlers.Authentication
{
    public class PlayerLogInHandler : PacketHandler
    {
        private readonly IAccountRepository accountRepository;
        private readonly ServerConfiguration serverConfiguration;
        private readonly Game game;
        private readonly PlayerLogInCommand playerLogInCommand;

        public PlayerLogInHandler(IAccountRepository repositoryNeo,
         Game game, ServerConfiguration serverConfiguration, PlayerLogInCommand playerLogInCommand)
        {
            this.accountRepository = repositoryNeo;
            this.game = game;
            this.serverConfiguration = serverConfiguration;
            this.playerLogInCommand = playerLogInCommand;
        }

        public override async void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            if (game.State == GameState.Stopped)
            {
                connection.Close();
            }

            var packet = new PlayerLogInPacket(message);

            connection.SetXtea(packet.Xtea);

            //todo linux os

            Verify(connection, packet);

            //todo: ip ban validation

            var playerRecord = await accountRepository.GetPlayer(packet.Account, packet.Password, packet.CharacterName);

            if (playerRecord is null)
            {
                connection.Send(new GameServerDisconnectPacket($"Account name or password is not correct."));
                return;
            }

            await accountRepository.Reload(playerRecord.GuildMember);
            await accountRepository.Reload(playerRecord);

         

            game.Dispatcher.AddEvent(new Event(() => playerLogInCommand.Execute(playerRecord, packet.CharacterName, connection)));
        }

        private void Verify(IConnection connection, PlayerLogInPacket packet)
        {

            if (string.IsNullOrWhiteSpace(packet.Account))
            {
                connection.Send(new GameServerDisconnectPacket("You must enter your account name."));
                return;
            }

            if (serverConfiguration.Version != packet.Version)
            {
                connection.Send(new GameServerDisconnectPacket($"Only clients with protocol {serverConfiguration.Version} allowed!"));
                return;
            }

            if (game.State == GameState.Opening)
            {
                connection.Send(new GameServerDisconnectPacket($"Gameworld is starting up. Please wait."));
                return;
            }
            if (game.State == GameState.Maintaining)
            {
                connection.Send(new GameServerDisconnectPacket($"Gameworld is under maintenance. Please re-connect in a while."));
                return;
            }

            if (game.State == GameState.Closed)
            {
                connection.Send(new GameServerDisconnectPacket("Server is currently closed.\nPlease try again later."));
                return;
            }
        }
    }
}
