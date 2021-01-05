using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Commands
{
    public class PlayerLogOutCommand : Command
    {
        private readonly Game game;
        private readonly IPlayer player;
        private readonly bool forced;
        public PlayerLogOutCommand(IPlayer player, Game game, bool forced = false)
        {
            this.player = player;
            this.game = game;
            this.forced = forced;
        }

        public override void Execute()
        {
            if (player.IsRemoved) return;

            if (!player.Logout(forced) && !forced) return;

            game.CreatureManager.RemovePlayer(player);
            player.SetAsRemoved();
            return;

        }
    }
}