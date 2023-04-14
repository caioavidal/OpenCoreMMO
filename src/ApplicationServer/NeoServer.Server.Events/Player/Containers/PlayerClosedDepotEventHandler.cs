using System.Collections.Generic;
using System.Threading.Tasks;
using NeoServer.Data.Interfaces;
using NeoServer.Data.Parsers;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
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
         Save((int)player.Id, depot.Items).Wait();
    }

    private async Task Save(int playerId, List<IItem> items, int parentId = 0)
    {
        foreach (var item in items)
        {
            var itemModel = ItemModelParser.ToModel(item);
            itemModel.PlayerId = playerId;
            itemModel.ParentId = parentId;
            await _playerDepotItemRepository.Insert(itemModel);

            if (item is IContainer container) await Save(playerId, container.Items, itemModel.Id);
        }
    }
}