using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Events
{
    public class PlayerClosedContainerEventHandler
    {
        private readonly IMap map;
        private readonly Game game;
        //private readonly IPlayerDepotRepository playerDepotRepository;

        public PlayerClosedContainerEventHandler(IMap map, Game game/*, IPlayerDepotRepository playerDepotRepository*/)
        {
            this.map = map;
            this.game = game;
            //this.playerDepotRepository = playerDepotRepository;
        }
        public void Execute(IPlayer player, byte containerId, IContainer container)
        {
            //if (game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection))
            //{
            //    connection.OutgoingPackets.Enqueue(new CloseContainerPacket(containerId));
            //    connection.Send();
            //}

            //if (container.Root is IDepot depot && player.HasDepotOpened is false)
            //{
            //    var items = new List<ItemModel>(depot.Items.Count);

            //    foreach (var item in depot.Items)
            //    {
            //        items.Add(ItemModelParser.ToModel(item));
            //    }
            //    playerDepotRepository.Save(new PlayerDepotModel(player.Id, items));
            //    depot.Clear();
            //}
        }
    }
}
