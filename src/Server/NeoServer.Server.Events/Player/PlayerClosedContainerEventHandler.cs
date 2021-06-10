using System.Collections.Generic;
using System.Threading.Tasks;
using NeoServer.Data.Interfaces;
using NeoServer.Data.Parsers;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Networking.Packets.Outgoing.Player;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Player
{
    public class PlayerClosedContainerEventHandler
    {
        private readonly IGameServer game;
        private readonly IMap map;
        private readonly IPlayerDepotItemRepositoryNeo playerDepotItemRepository;

        public PlayerClosedContainerEventHandler(IMap map, IGameServer game,
            IPlayerDepotItemRepositoryNeo playerDepotItemRepository)
        {
            this.map = map;
            this.game = game;
            this.playerDepotItemRepository = playerDepotItemRepository;
        }

        public async void Execute(IPlayer player, byte containerId, IContainer container)
        {
            if (game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection))
            {
                connection.OutgoingPackets.Enqueue(new CloseContainerPacket(containerId));
                connection.Send();
            }

            if (container.RootParent is IDepot depot && player.HasDepotOpened is false)
            {
                //todo: process very expensive. need to find another solution
                await playerDepotItemRepository.DeleteAll(player.Id);
                await Save((int) player.Id, depot.Items);
                depot.Clear();
            }
        }

        private async Task Save(int playerId, List<IItem> items, int parentId = 0)
        {
            foreach (var item in items)
            {
                var itemModel = ItemModelParser.ToModel(item);
                itemModel.PlayerId = playerId;
                itemModel.ParentId = parentId;
                await playerDepotItemRepository.Insert(itemModel);

                if (item is IContainer container) await Save(playerId, container.Items, itemModel.Id);
            }
        }
    }
}