using NeoServer.Data.Interfaces;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Parsers;
using NeoServer.Networking.Packets.Outgoing.Creature;
using NeoServer.Networking.Packets.Outgoing.Effect;
using NeoServer.Networking.Packets.Outgoing.Map;
using NeoServer.Networking.Packets.Outgoing.Player;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Server.Events.Player
{
    public class PlayerLoggedOutEventHandler : IEventHandler
    {
        private readonly IAccountRepository _accountRepository;

        public PlayerLoggedOutEventHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async void Execute(IPlayer player) => await _accountRepository.UpdatePlayerOnlineStatus(player.Id, false);
    }
}