using NeoServer.Data.Interfaces;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Creatures;
using NeoServer.Loaders.Interfaces;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Commands;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Standalone;
using NeoServer.Server.Tasks;
using System.Collections.Generic;

namespace NeoServer.Server.Handlers.Authentication
{
    public class PlayerLogInHandler : PacketHandler
    {
        //private readonly IAccountRepository repository;
        private readonly IAccountRepositoryNeo repositoryNeo;
        private readonly ServerConfiguration serverConfiguration;
        private readonly Game game;
        private CreaturePathAccess _creaturePathAccess;
        private readonly IItemFactory _itemFactory;
        private readonly IEnumerable<IPlayerLoader> playerLoaders;

        public PlayerLogInHandler(/*IAccountRepository repository,*/ IAccountRepositoryNeo repositoryNeo,
         Game game, ServerConfiguration serverConfiguration, CreaturePathAccess creaturePathAccess, IItemFactory itemFactory, IEnumerable<IPlayerLoader> playerLoaders)
        {
            this.repositoryNeo = repositoryNeo;
            // this.repository = repository;
            this.game = game;
            this.serverConfiguration = serverConfiguration;
            _creaturePathAccess = creaturePathAccess;
            _itemFactory = itemFactory;
            this.playerLoaders = playerLoaders;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
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

            //var accountRecord = repository.FirstOrDefault(a => a.AccountName == packet.Account && a.Password == packet.Password);
            var accountRecord = repositoryNeo.Login(packet.Account, packet.Password).Result;

            if (accountRecord == null)
            {
                connection.Send(new GameServerDisconnectPacket($"Account name or password is not correct."));
                return;
            }

            game.Dispatcher.AddEvent(new Event(new PlayerLogInCommand(accountRecord, packet.CharacterName, game, connection, playerLoaders).Execute));
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
