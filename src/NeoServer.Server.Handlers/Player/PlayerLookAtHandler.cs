using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Contracts.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Handlers.Player
{
    public class PlayerLookAtHandler : PacketHandler
    {
        private readonly Game game;
        public PlayerLookAtHandler(Game game)
        {
            this.game = game;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var lookAtPacket = new LookAtPacket(message);

            if (game.Map[lookAtPacket.Location] is IWalkableTile tile)
            {
                var thingId = tile.GetThingByStackPosition(lookAtPacket.StackPosition);
                Console.WriteLine(thingId);
            }
        }
    }
}
