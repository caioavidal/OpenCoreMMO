using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Commands;
using NeoServer.Server.Contracts.Commands;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players;
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
            if (!player.IsRemoved)
            {
                game.CreatureManager.RemovePlayer(player);
                return;
            }

            if (forced)
            {
                return;
            }

            if (game.CreatureManager.GetPlayerConnection(player.CreatureId, out IConnection connection))
            {
                if (player.Tile.CannotLogout)
                {
                    connection.Send(new TextMessagePacket("You can not logout here.", TextMessageOutgoingType.MESSAGE_STATUS_SMALL));
                    return;
                }

                if (player.CannotLogout)
                {
                    connection.Send(new TextMessagePacket("You may not logout during or immediately after a fight!", TextMessageOutgoingType.MESSAGE_STATUS_SMALL));
                    return;
                }
            }
        }
    }
}