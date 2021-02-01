using Autofac;
using NeoServer.Loaders.Attributes;
using NeoServer.Loaders.Interfaces;
using NeoServer.Server.Contracts.Commands;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Server.Commands
{
    public class PlayerLogInCommand: ICommand
    {
        private readonly Game game;
        private readonly IEnumerable<IPlayerLoader> playerLoaders;

        public PlayerLogInCommand(Game game, IEnumerable<IPlayerLoader> playerLoaders)
        {
            this.game = game;
            this.playerLoaders = playerLoaders;
        }

        public void Execute(PlayerModel playerRecord, string characterName, IConnection connection)
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