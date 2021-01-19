using NeoServer.Data.Interfaces;
using NeoServer.Data.Parsers;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.Items.Types.Containers;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Model.Players.Contracts;
using System.Linq;

namespace NeoServer.Server.Events
{
    public class PlayerOpenedContainerEventHandler
    {
        private readonly Game game;
        private readonly IPlayerDepotItemRepositoryNeo playerDepotItemRepository;

        private readonly IItemFactory itemFactory;
        public PlayerOpenedContainerEventHandler(Game game, IPlayerDepotItemRepositoryNeo playerDepotItemRepository, IItemFactory itemFactory)
        {
            this.game = game;
            this.playerDepotItemRepository = playerDepotItemRepository;
            this.itemFactory = itemFactory;
        }
        public async void Execute(IPlayer player, byte containerId, IContainer container)
        {
            if (container is IDepot && !container.HasItems)
            {
                var records = await playerDepotItemRepository.GetByPlayerId(player.Id); //todo
                ItemModelParser.BuildContainer(records.Where(c => c.ParentId.Equals(0)).ToList(), 0, container.Location, container, itemFactory, records.ToList());
            }

            if (game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection))
            {
                connection.OutgoingPackets.Enqueue(new OpenContainerPacket(container, containerId));
                connection.Send();
            }
        }
    }
}
