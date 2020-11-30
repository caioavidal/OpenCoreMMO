using NeoServer.Data.Parsers;
using NeoServer.Data.Repositories;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.Items.Types.Containers;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Model.Players.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeoServer.Server.Events
{
    public class PlayerOpenedContainerEventHandler
    {
        private readonly Game game;
        private readonly IPlayerDepotRepository playerDepotRepository;
        private readonly IItemFactory itemFactory;
        public PlayerOpenedContainerEventHandler(Game game, IPlayerDepotRepository playerDepotRepository, IItemFactory itemFactory)
        {
            this.game = game;
            this.playerDepotRepository = playerDepotRepository;
            this.itemFactory = itemFactory;
        }
        public void Execute(IPlayer player, byte containerId, IContainer container)
        {
            if (container is IDepot && !container.HasItems)
            {
                var records = playerDepotRepository.Get(player.Id); //todo
                ItemModelParser.BuildContainer(records.Items, 0, container.Location, container, itemFactory);
            }

            if (game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection))
            {
                connection.OutgoingPackets.Enqueue(new OpenContainerPacket(container, containerId));
                connection.Send();
            }
        }
    }
}
