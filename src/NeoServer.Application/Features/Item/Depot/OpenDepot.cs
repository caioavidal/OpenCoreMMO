using Mediator;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Infrastructure.Data.Interfaces;
using NeoServer.Infrastructure.Data.Parsers;

namespace NeoServer.Application.Features.Item.Depot;

public record OpenDepotCommand(IPlayer Player, IDepot Depot, Location Location, byte Index) : ICommand;

public class OpenDepotCommandHandler : ICommandHandler<OpenDepotCommand>
{
    private readonly DepotTracker _depotTracker;
    private readonly IItemFactory _itemFactory;
    private readonly IPlayerDepotItemRepository _playerDepotItemRepository;
    private readonly IPlayerUseService _playerUseService;

    public OpenDepotCommandHandler(IPlayerUseService playerUseService,
        IPlayerDepotItemRepository playerDepotItemRepository, IItemFactory itemFactory, DepotTracker depotTracker)
    {
        _playerUseService = playerUseService;
        _playerDepotItemRepository = playerDepotItemRepository;
        _itemFactory = itemFactory;
        _depotTracker = depotTracker;
    }

    public ValueTask<Unit> Handle(OpenDepotCommand command, CancellationToken cancellationToken)
    {
        command.Deconstruct(out var player, out var depot, out var location, out var index);
        var playerDepot = LoadDepot(player, depot);

        playerDepot.SetNewLocation(location);

        _playerUseService.Use(player, playerDepot, index);

        return Unit.ValueTask;
    }

    private IDepot LoadDepot(IPlayer player, IItem container)
    {
        var depot = _depotTracker.Get(player.Id);
        if (depot is not null) return depot;

        var depotRecordsTask = _playerDepotItemRepository.GetByPlayerId(player.Id);

        depot = (IDepot)_itemFactory.Create(container.Metadata, container.Location, null);

        var depotRecords = depotRecordsTask.Result.ToList();

        var depotItemModels = depotRecords.ToList();

        ItemEntityParser.BuildContainer(depot, depotItemModels, container.Location, _itemFactory);

        _depotTracker.Load(player.Id, depot);

        return depot;
    }
}