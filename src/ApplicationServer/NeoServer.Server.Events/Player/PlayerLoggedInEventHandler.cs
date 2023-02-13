using NeoServer.Data.Interfaces;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Player;

public class PlayerLoggedInEventHandler : IEventHandler
{
    private readonly IAccountRepository _accountRepository;

    public PlayerLoggedInEventHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async void Execute(IWalkableCreature creature)
    {
        if (creature.IsNull()) return;

        if (creature is not IPlayer player) return;

        await _accountRepository.UpdatePlayerOnlineStatus(player.Id, true);
    }
}