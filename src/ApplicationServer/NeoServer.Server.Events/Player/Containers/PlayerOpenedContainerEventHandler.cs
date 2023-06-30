using System.Linq;
using NeoServer.Data.Interfaces;
using NeoServer.Data.Parsers;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Systems.Depot;
using NeoServer.Networking.Packets.Outgoing.Player;
using NeoServer.Server.Common.Contracts;
using Serilog;

namespace NeoServer.Server.Events.Player.Containers;

public class PlayerOpenedContainerEventHandler
{
    private readonly IGameServer game;

    private readonly IItemFactory itemFactory;
    private readonly DepotManager _depotManager;
    private readonly ILogger _logger;
    private readonly IPlayerDepotItemRepository playerDepotItemRepository;

    public PlayerOpenedContainerEventHandler(IGameServer game,
        IPlayerDepotItemRepository playerDepotItemRepository, IItemFactory itemFactory,
        DepotManager depotManager, ILogger logger)
    {
        this.game = game;
        this.playerDepotItemRepository = playerDepotItemRepository;
        this.itemFactory = itemFactory;
        _depotManager = depotManager;
        _logger = logger;
    }

    public void Execute(IPlayer player, byte containerId, IContainer container)
    {
       
            SendContainerPacket(player, containerId, container);
            return;


            // if (container.HasItems)
            // {
            //     _logger.Information("{Player} tried to open depot that contains items", player.Name);
            //     return;
            // }
            //
            // var depot = _depotManager.Get(player.Id);
            // if (depot is not null)
            // {
            //     SendContainerPacket(player, containerId, depot);
            //     return;
            // }
            //
            // depot = LoadDepotFromRecords(player, container);
            // SendContainerPacket(player, containerId, depot);
    }

    private IDepot LoadDepotFromRecords(IPlayer player, IItem container)
    {
        var depotRecordsTask = playerDepotItemRepository.GetByPlayerId(player.Id);

        var depot = (IDepot)itemFactory.Create(container.Metadata, container.Location, null);

        var depotRecords = depotRecordsTask.Result.ToList();

        var depotItemModels = depotRecords.Where(record => record.ParentId.Equals(0)).ToList();

        ItemModelParser.BuildContainer(depotItemModels, depotItemModels.Count - 1, container.Location, depot, itemFactory, depotRecords);

        _depotManager.Load(player.Id, depot);

        return depot;
    }

    private void SendContainerPacket(IPlayer player, byte containerId, IContainer container)
    {
        if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;

        connection.OutgoingPackets.Enqueue(new OpenContainerPacket(container, containerId));
        connection.Send();
    }
}