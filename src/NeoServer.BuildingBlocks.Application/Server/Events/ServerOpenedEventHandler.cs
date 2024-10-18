using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.BuildingBlocks.Application.Contracts.Repositories;

namespace NeoServer.BuildingBlocks.Application.Server.Events;

public class ServerOpenedEventHandler : IEventHandler
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