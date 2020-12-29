using NeoServer.Data.Model;
using NeoServer.Game.Contracts;
using NeoServer.Server.Contracts.Network;
using System.Linq;
using System.Threading.Tasks;
using NeoServer.Server.Model.Players;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures;
using NeoServer.Game.Creature.Model;
using System.Collections.Generic;
using NeoServer.Game.Common.Players;
using NeoServer.Game.Contracts.Items.Types;
using System;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Common;
using NeoServer.Loaders.Players;

namespace NeoServer.Server.Commands
{
    public class PlayerLogInCommand : Command
    {
        private readonly AccountModel account;
        private readonly string characterName;
        private readonly Game game;
        private readonly IConnection connection;
        private readonly PlayerLoader playerLoader;

        public PlayerLogInCommand(AccountModel account, string characterName, Game game, IConnection connection, PlayerLoader playerLoader)
        {
            this.account = account;
            this.characterName = characterName;

            this.game = game;
            this.connection = connection;
            this.playerLoader = playerLoader;
        }

        public override void Execute()
        {
            var playerRecord = account.Players.FirstOrDefault(p => p.Name.Equals(characterName));

            if (playerRecord == null)
            {
                //todo validations here
                return;
            }

            var player = playerLoader.Load(playerRecord);

            game.CreatureManager.AddPlayer(player, connection);
        }
    }
}