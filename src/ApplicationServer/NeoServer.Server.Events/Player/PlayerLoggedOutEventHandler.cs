using NeoServer.Data.Interfaces;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Player;

public class PlayerLoggedOutEventHandler : IEventHandler
{
    private readonly IPlayerRepository _playerRepository;

    public PlayerLoggedOutEventHandler(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public void Execute(IPlayer player)
    {
        _playerRepository.UpdatePlayer(player).Wait();
        _playerRepository.SavePlayerInventory(player).Wait();
        _playerRepository.UpdatePlayerOnlineStatus(player.Id, false).Wait();
        _playerRepository.SaveBackpack(player).Wait();
    }
}