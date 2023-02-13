using System.Linq;
using NeoServer.Data.Interfaces;
using NeoServer.Data.Parsers;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Networking.Packets.Outgoing.Player;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Player.Containers;

public class PlayerOpenedContainerEventHandler
{
    private readonly IGameServer game;

    private readonly IItemFactory itemFactory;
    private readonly IPlayerDepotItemRepository playerDepotItemRepository;

    public PlayerOpenedContainerEventHandler(IGameServer game,
        IPlayerDepotItemRepository playerDepotItemRepository, IItemFactory itemFactory)
    {
        this.game = game;
        this.playerDepotItemRepository = playerDepotItemRepository;
        this.itemFactory = itemFactory;
    }

    public async void Execute(IPlayer player, byte containerId, IContainer container)
    {
        if (container is IDepot && !container.HasItems)
        {
            var records = (await playerDepotItemRepository.GetByPlayerId(player.Id)).ToList(); //todo
            ItemModelParser.BuildContainer(records.Where(c => c.ParentId.Equals(0)).ToList(), 0, container.Location,
                container, itemFactory, records.ToList());
        }

        if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;

        connection.OutgoingPackets.Enqueue(new OpenContainerPacket(container, containerId));
        connection.Send();
    }
}