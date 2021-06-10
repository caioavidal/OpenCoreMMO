using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Chat
{
    public class PlayerCloseNpcChannelHandler : PacketHandler
    {
        private readonly IGameServer game;

        public PlayerCloseNpcChannelHandler(IGameServer game)
        {
            this.game = game;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

            foreach (var creature in game.Map.GetCreaturesAtPositionZone(player.Location))
                if (creature is INpc npc)
                    game.Dispatcher.AddEvent(new Event(() => npc.StopTalkingToCustomer(player)));
        }
    }
}