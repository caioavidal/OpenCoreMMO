using NeoServer.Data.Interfaces;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Server.Events.Player
{
    public class PlayerAddToVipListEventHandler
    {
        private readonly Game game;
        private readonly IAccountRepository accountRepository;
        public PlayerAddToVipListEventHandler(Game game, IAccountRepository accountRepository)
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
