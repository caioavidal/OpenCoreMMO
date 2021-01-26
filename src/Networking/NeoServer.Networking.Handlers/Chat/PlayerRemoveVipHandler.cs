using NeoServer.Data.Interfaces;
using NeoServer.Game.Contracts.Chats;
using NeoServer.Game.DataStore;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Incoming.Chat;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;
using System.Linq;

namespace NeoServer.Server.Handlers.Player
{
    public class PlayerRemoveVipHandler : PacketHandler
    {
        private readonly Game game;
        private readonly IAccountRepository accountRepository;
        public PlayerRemoveVipHandler(Game game, IAccountRepository accountRepository)
        {
            this.game = game;
            this.accountRepository = accountRepository;
        }
        public override async void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            if (!game.CreatureManager.TryGetPlayer(connection.PlayerId, out var player)) return;

            var removeVipPacket = new RemoveVipPacket(message);
            

            game.Dispatcher.AddEvent(new Event(() => player.RemoveFromVip(removeVipPacket.PlayerId)));
        }
    }
}

