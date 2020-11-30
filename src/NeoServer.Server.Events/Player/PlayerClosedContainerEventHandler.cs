using NeoServer.Data.Model;
using NeoServer.Data.Parsers;
using NeoServer.Data.Repositories;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.Items.Types.Containers;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Server.Events
{
    public class PlayerClosedContainerEventHandler
    {
        private readonly IMap map;
        private readonly Game game;
        private readonly IPlayerDepotRepository playerDepotRepository;

        public PlayerClosedContainerEventHandler(IMap map, Game game, IPlayerDepotRepository playerDepotRepository)
        {
            this.map = map;
            this.game = game;
            this.playerDepotRepository = playerDepotRepository;
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
                var items = new List<IItemModel>(depot.Items.Count);

                foreach (var item in depot.Items)
                {
                    items.Add(ItemModelParser.ToModel(item));
                }
                playerDepotRepository.Save(new PlayerDepotModel(player.Id, items));
                depot.Clear();
            }
        }
    }
}
