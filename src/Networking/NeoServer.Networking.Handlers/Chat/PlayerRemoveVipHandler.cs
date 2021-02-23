using NeoServer.Data.Interfaces;
using NeoServer.Networking.Packets.Incoming.Chat;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Handlers.Player
{
    public class PlayerRemoveVipHandler : PacketHandler
    {
        private readonly IGameServer game;
        private readonly IAccountRepository accountRepository;
        public PlayerRemoveVipHandler(IGameServer game, IAccountRepository accountRepository)
        {
            this.game = game;
            this.accountRepository = accountRepository;
        }
        public override async void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

            var removeVipPacket = new RemoveVipPacket(message);
            

            game.Dispatcher.AddEvent(new Event(() => player.RemoveFromVip(removeVipPacket.PlayerId)));

            await accountRepository.RemoveFromVipList((int)player.AccountId, (int)removeVipPacket.PlayerId);
        }
    }
}

