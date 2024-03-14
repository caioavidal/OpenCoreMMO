using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Common.Contracts.Repositories;
using NeoServer.Application.Features.Item.Depot;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Application.Features.Session.LogOut;

public class PlayerLoggedOutEventHandler : IEventHandler
{
    private readonly DepotManager _depotManager;
    private readonly IGameServer _gameServer;
    private readonly IPlayerDepotItemRepository _playerDepotItemRepository;
    private readonly IPlayerRepository _playerRepository;

    public PlayerLoggedOutEventHandler(IPlayerRepository playerRepository,
        IPlayerDepotItemRepository playerDepotItemRepository,
        DepotManager depotManager,
        IGameServer gameServer)
    {
        _playerRepository = playerRepository;
        _playerDepotItemRepository = playerDepotItemRepository;
        _depotManager = depotManager;
        _gameServer = gameServer;
    }

    public void Execute(IPlayer player)
    {
        SavePlayer(player);
        _gameServer.CreatureManager.RemovePlayer(player);
    }

    private void SavePlayer(IPlayer player)
    {
        _playerRepository.SavePlayer(player);
        _playerRepository.UpdatePlayerOnlineStatus(player.Id, false).Wait();
        SaveDepot(player);
    }

    private void SaveDepot(IPlayer player)
    {
        if (!_depotManager.Get(player.Id, out var depot)) return;
        _playerDepotItemRepository.Save(player, depot).Wait();

        _depotManager.Unload(player.Id);
    }
}