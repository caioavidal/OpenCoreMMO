using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Commands;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Commands
{
    public class PlayerLogOutCommand : ICommand
    {
        private readonly IGameServer game;
     
        public PlayerLogOutCommand(IGameServer game)
        {
            this.game = game;
        }

        public void Execute(IPlayer player, bool forced = false)
        {
            if (player.IsRemoved) return;

            if (!player.Logout(forced) && !forced) return;

            game.CreatureManager.RemovePlayer(player);
            player.SetAsRemoved();
            return;

        }
    }
}