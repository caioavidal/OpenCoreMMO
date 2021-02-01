using NeoServer.Data.Model;
using NeoServer.Loaders.Interfaces;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Server.Commands
{
    public class PlayerLogInCommand : Command
    {
        private readonly PlayerModel playerRecord;
        private readonly string characterName;
        private readonly Game game;
        private readonly IConnection connection;
        private readonly IEnumerable<IPlayerLoader> playerLoaders;

        public PlayerLogInCommand(PlayerModel player, string characterName, Game game, IConnection connection, IEnumerable<IPlayerLoader> playerLoader)
        {
            this.playerRecord = player;
            this.characterName = characterName;

            this.game = game;
            this.connection = connection;
            this.playerLoaders = playerLoader;
        }

        public override void Execute()
        {
            if (playerRecord is null)
            {
                //todo validations here
                return;
            }

            if (!game.CreatureManager.TryGetLoggedPlayer((uint)playerRecord.PlayerId, out var player))
            {
                if (playerLoaders.FirstOrDefault(x => x.IsApplicable(playerRecord)) is not IPlayerLoader playerLoader) return;

                player = playerLoader.Load(playerRecord);
            }

            game.CreatureManager.AddPlayer(player, connection);

            player.Login();
            player.LoadVipList(playerRecord.Account.VipList.Select(x => ((uint)x.PlayerId, x.Player?.Name)));
        }
    }
}