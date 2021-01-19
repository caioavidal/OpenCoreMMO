using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Tasks;
using System;

namespace NeoServer.Server.Handlers.Player
{
    public sealed class PlayerPingResponseHandler : PacketHandler
    {
        private readonly Game game;
        public PlayerPingResponseHandler(Game game)
        {
            this.game = game;
        }
        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            game.Dispatcher.AddEvent(new Event(() => connection.LastPingResponse = DateTime.Now.Ticks));
        }
    }
}
