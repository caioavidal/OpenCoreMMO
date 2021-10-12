using NeoServer.Data.Interfaces;
using NeoServer.Networking.Packets.Incoming.Chat;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Chat
{
    public class PlayerRemoveVipHandler : PacketHandler
    {
        private readonly IAccountRepository accountRepository;
        private readonly IGameServer game;

        public PlayerRemoveVipHandler(IGameServer game, IAccountRepository accountRepository)
        {
            this.game = game;
            this.accountRepository = accountRepository;
        }

        public override async void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

            var removeVipPacket = new RemoveVipPacket(message);

            game.Dispatcher.AddEvent(new Event(() => player.Vip.RemoveFromVip(removeVipPacket.PlayerId)));

            await accountRepository.RemoveFromVipList((int) player.AccountId, (int) removeVipPacket.PlayerId);
        }
    }
}