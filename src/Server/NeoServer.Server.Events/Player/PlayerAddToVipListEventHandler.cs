using NeoServer.Data.Interfaces;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Server.Events.Player
{
    public class PlayerAddToVipListEventHandler
    {
        private readonly IGameServer game;
        private readonly IAccountRepository accountRepository;
        public PlayerAddToVipListEventHandler(IGameServer game, IAccountRepository accountRepository)
        {
            this.game = game;
            this.accountRepository = accountRepository;
        }

        public async void Execute(IPlayer player, uint vipPlayerId, string vipPlayerName)
        {
            if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;

            await accountRepository.AddPlayerToVipList((int)player.AccountId, (int)vipPlayerId);

            var isOnline = game.CreatureManager.TryGetLoggedPlayer(vipPlayerId, out var loggedPlayer);

            connection.OutgoingPackets.Enqueue(new PlayerAddVipPacket(vipPlayerId, vipPlayerName, isOnline));
            connection.Send();

        }
    }
}
