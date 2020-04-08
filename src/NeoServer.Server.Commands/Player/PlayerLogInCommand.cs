using NeoServer.Data.Model;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Commands;
using NeoServer.Server.Contracts.Commands;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Contracts.Repositories;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks.Contracts;
using System.Linq;

namespace NeoServer.Server.Commands
{
    public class PlayerLogInCommand : Command
    {
        private readonly AccountModel account;
        private readonly string characterName;
        private readonly Game game;
        private readonly IConnection connection;


        public PlayerLogInCommand(AccountModel account, string characterName, Game game, IConnection connection)
        {
            this.account = account;
            this.characterName = characterName;

            this.game = game;
            this.connection = connection;
        }


        public override async void Execute()
        {
            var playerRecord = account.Players.FirstOrDefault(p => p.CharacterName == characterName);

            game.CreatureManager.AddPlayer(playerRecord, connection);
        }
    }
}