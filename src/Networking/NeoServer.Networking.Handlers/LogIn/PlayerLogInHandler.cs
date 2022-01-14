using NeoServer.Data.Interfaces;
using NeoServer.Data.Model;
using NeoServer.Game.Common;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Commands.Player;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Common.Enums;
using NeoServer.Server.Configurations;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.LogIn
{
    public class PlayerLogInHandler : PacketHandler
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IGameServer _game;
        private readonly PlayerLogInCommand _playerLogInCommand;
        private readonly ServerConfiguration _serverConfiguration;

        public PlayerLogInHandler(IAccountRepository repositoryNeo,
            IGameServer game, ServerConfiguration serverConfiguration, PlayerLogInCommand playerLogInCommand)
        {
            _accountRepository = repositoryNeo;
            _game = game;
            _serverConfiguration = serverConfiguration;
            _playerLogInCommand = playerLogInCommand;
        }

        public override async void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            if (_game.State == GameState.Stopped) connection.Close();

            var packet = new PlayerLogInPacket(message);

            connection.SetXtea(packet.Xtea);

            //todo linux os

            if (!Verify(connection, packet)) return;

            //todo: ip ban validation

            var playerOnline = await _accountRepository.GetOnlinePlayer(packet.Account);

            if (ValidateOnlineStatus(connection, playerOnline, packet).Failed) return;

            var playerRecord =
                await _accountRepository.GetPlayer(packet.Account, packet.Password, packet.CharacterName);

            if (playerRecord is null)
            {
                Disconnect(connection, "Account name or password is not correct.");
                return;
            }

            _game.Dispatcher.AddEvent(new Event(() => _playerLogInCommand.Execute(playerRecord, connection)));
        }

        private static Result ValidateOnlineStatus(IConnection connection, PlayerModel playerOnline,
            PlayerLogInPacket packet)
        {
            if (playerOnline is null) return Result.Success;

            if (playerOnline?.Name == packet.CharacterName)
            {
                Disconnect(connection, "You are already logged in.");
                return Result.NotPossible;
            }

            if (playerOnline.Account.AllowManyOnline) return Result.Success;
            
            Disconnect(connection, "You may only login with one character of your account at the same time.");
            return Result.NotPossible;
        }

        private bool Verify(IConnection connection, PlayerLogInPacket packet)
        {
            if (string.IsNullOrWhiteSpace(packet.Account))
            {
                Disconnect(connection, "You must enter your account name.");
                return false;
            }

            if (_serverConfiguration.Version != packet.Version)
            {
                Disconnect(connection, $"Only clients with protocol {_serverConfiguration.Version} allowed!");
                return false;
            }

            switch (_game.State)
            {
                case GameState.Opening:
                    Disconnect(connection, "Gameworld is starting up. Please wait.");
                    return false;
                case GameState.Maintaining:
                    Disconnect(connection, "Gameworld is under maintenance. Please re-connect in a while.");
                    return false;
                case GameState.Closed:
                    Disconnect(connection, "Server is currently closed.\nPlease try again later.");
                    return false;
            }

            return true;
        }

        private static void Disconnect(IConnection connection, string message)
        {
            connection.Send(new GameServerDisconnectPacket(message));
            connection.Close();
        }
    }
}