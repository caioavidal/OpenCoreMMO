using NeoServer.Data.Interfaces;
using NeoServer.Data.Model;
using NeoServer.Data.Parsers;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.Items.Types.Containers;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Model.Players.Contracts;
using System.Collections.Generic;

namespace NeoServer.Server.Events
{
    public class PlayerClosedContainerEventHandler
    {
        private readonly IMap map;
        private readonly Game game;
        private readonly IPlayerDepotItemRepositoryNeo playerDepotItemRepository;

        public PlayerClosedContainerEventHandler(IMap map, Game game, IPlayerDepotItemRepositoryNeo playerDepotItemRepository)
        {
            this.map = map;
            this.game = game;
            this.playerDepotItemRepository = playerDepotItemRepository;
        }
        public void Execute(IPlayer player, byte containerId, IContainer container)
        {
            if (game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection))
            {
                connection.OutgoingPackets.Enqueue(new CloseContainerPacket(containerId));
                connection.Send();
            }

            if (container.Root is IDepot depot && player.HasDepotOpened is false)
            {
                foreach (var item in depot.Items)
                {
                    var itemModel = ItemModelParser.ToModel(item);
                    playerDepotItemRepository.Insert(itemModel);
                }
                depot.Clear();
            }
        }
    }
}
