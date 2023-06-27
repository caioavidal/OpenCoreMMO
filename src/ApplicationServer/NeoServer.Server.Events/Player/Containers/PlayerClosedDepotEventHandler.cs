using NeoServer.Data.Interfaces;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;

namespace NeoServer.Server.Events.Player.Containers;

public class PlayerClosedDepotEventHandler
{
    private readonly IPlayerDepotItemRepository _playerDepotItemRepository;

    public PlayerClosedDepotEventHandler(IPlayerDepotItemRepository playerDepotItemRepository)
    {
        _playerDepotItemRepository = playerDepotItemRepository;
    }

    public void Execute(IPlayer player, byte containerId, IDepot container)
    {
        if (container.RootParent is not IDepot depot || player.HasDepotOpened) return;

        //todo: process very expensive. need to find another solution
        _playerDepotItemRepository.DeleteAll(player.Id).Wait();
        _playerDepotItemRepository.Save((int)player.Id, depot).Wait();
    }
}