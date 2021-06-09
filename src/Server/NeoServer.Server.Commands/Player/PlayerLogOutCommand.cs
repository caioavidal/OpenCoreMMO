using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Commands;

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
            if (!player.Logout(forced) && !forced) return;

            game.CreatureManager.RemovePlayer(player);
        }
    }
}