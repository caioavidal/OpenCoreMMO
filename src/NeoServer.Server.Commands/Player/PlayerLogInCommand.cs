using NeoServer.Data.Model;
using NeoServer.Game.Contracts;
using NeoServer.Server.Contracts.Network;
using System.Linq;

namespace NeoServer.Server.Commands
{
    public class PlayerLogInCommand : Command
    {
        private readonly IAccountModel account;
        private readonly string characterName;
        private readonly Game game;
        private readonly IConnection connection;

        public PlayerLogInCommand(IAccountModel account, string characterName, Game game, IConnection connection)
        {
            this.account = account;
            this.characterName = characterName;

            this.game = game;
            this.connection = connection;
        }

        public override async void Execute()
        {
            var playerRecord = account.Players.FirstOrDefault(p => p.CharacterName == characterName);

            if (playerRecord == null)
            {
                //todo validations here
                return;
            }

            game.CreatureManager.AddPlayer(playerRecord, connection);
        }
    }
}