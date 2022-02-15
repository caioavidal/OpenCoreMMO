using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Helpers;
using NeoServer.Networking.Packets.Outgoing.Npc;
using NeoServer.Networking.Packets.Outgoing.Player;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Player;

public class PlayerChangedInventoryEventHandler
{
    private readonly ICoinTypeStore _coinTypeStore;
    private readonly IGameServer game;

    public PlayerChangedInventoryEventHandler(IGameServer game, ICoinTypeStore coinTypeStore)
    {
        this.game = game;
        _coinTypeStore = coinTypeStore;
    }

    public void Execute(IInventory inventory, IPickupable item, Slot slot, byte amount = 1)
    {
        if (Guard.AnyNull(inventory)) return;

        var player = inventory.Owner;
        
        if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;
        
        connection.OutgoingPackets.Enqueue(new PlayerInventoryItemPacket(player.Inventory, slot));

        if (player.Shopping)
            connection.OutgoingPackets.Enqueue(new SaleItemListPacket(player,
                player.TradingWithNpc?.ShopItems?.Values, _coinTypeStore));

        connection.Send();
    }
}