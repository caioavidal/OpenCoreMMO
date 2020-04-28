using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Handlers.Player
{
    public class PlayerCloseContainerHandler : PacketHandler
    {
        private readonly Game game;
        public PlayerCloseContainerHandler(Game game)
        {
            this.game = game;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var containerId = message.GetByte();
            if (game.CreatureManager.TryGetCreature(connection.PlayerId, out ICreature player))
            {
                game.Dispatcher.AddEvent(new Event(() =>
                {
                    (player as IPlayer).Containers.CloseContainer(containerId);
                }));
            }
        }
    }
}
