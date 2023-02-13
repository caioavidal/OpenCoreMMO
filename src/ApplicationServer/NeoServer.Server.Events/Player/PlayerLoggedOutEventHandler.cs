using NeoServer.Data.Interfaces;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Player;

public class PlayerLoggedOutEventHandler : IEventHandler
{
    private readonly IAccountRepository _accountRepository;

    public PlayerLoggedOutEventHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async void Execute(IPlayer player)
    {
        await _accountRepository.UpdatePlayerOnlineStatus(player.Id, false);
        await _accountRepository.UpdatePlayer(player);
        await _accountRepository.SavePlayerInventory(player);
    }
}