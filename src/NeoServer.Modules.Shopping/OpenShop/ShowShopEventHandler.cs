using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Networking.Packets.Outgoing.Npc;

namespace NeoServer.Modules.Shopping.OpenShop;

public class ShowShopEventHandler : IEventHandler
{
    private readonly ICoinTypeStore _coinTypeStore;
    private readonly IGameServer _game;

    public ShowShopEventHandler(IGameServer game, ICoinTypeStore coinTypeStore)
    {
        _game = game;
        _coinTypeStore = coinTypeStore;
    }

    public void Execute(INpc npc, ISociableCreature to, IEnumerable<IShopItem> shopItems)
    {
        if (!_game.CreatureManager.GetPlayerConnection(to.CreatureId, out var connection)) return;

        shopItems = shopItems.ToList();

        connection.OutgoingPackets.Enqueue(new OpenShopPacket(shopItems));

        if (to is IPlayer { Shopping: true } player)
            connection.OutgoingPackets.Enqueue(new SaleItemListPacket(player, shopItems, _coinTypeStore));
        connection.Send();
    }
}