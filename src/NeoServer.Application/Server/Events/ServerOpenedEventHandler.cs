using NeoServer.Infrastructure.Data.Interfaces;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Application.Server.Events;

public class ServerOpenedEventHandler: IEventHandler
{
    private readonly IPlayerRepository _playerRepository;

    public ServerOpenedEventHandler(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public void Execute()
    {
        _playerRepository.UpdateAllPlayersToOffline();
    }
}