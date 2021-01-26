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
    public class PlayerAddVipHandler : PacketHandler
    {
        private readonly Game game;
        private readonly IAccountRepository accountRepository;
        public PlayerAddVipHandler(Game game, IAccountRepository accountRepository)
        {
            this.game = game;
            this.accountRepository = accountRepository;
        }
        public override async void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var addVipPacket = new AddVipPacket(message);

            if (addVipPacket.Name?.Length > 20) return;
            if (!game.CreatureManager.TryGetPlayer(connection.PlayerId, out var player)) return;

            var vipPlayerModel = await accountRepository.GetPlayer(addVipPacket.Name);

            if (vipPlayerModel is null)
            {
                connection.Send(new TextMessagePacket("A player with this name does not exist.", TextMessageOutgoingType.Small));
                return;
            }
            //todo: check if player can be added to vip list

            game.Dispatcher.AddEvent(new Event(() => player.AddToVip((uint)vipPlayerModel.PlayerId, vipPlayerModel.Name)));
        }
    }
}

