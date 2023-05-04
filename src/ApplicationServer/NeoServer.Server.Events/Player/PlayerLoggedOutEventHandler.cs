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

    public void Execute(IPlayer player)
    {
        _accountRepository.UpdatePlayerOnlineStatus(player.Id, false).Wait();
        _accountRepository.UpdatePlayer(player).Wait();
        _accountRepository.SavePlayerInventory(player).Wait();
    }
}