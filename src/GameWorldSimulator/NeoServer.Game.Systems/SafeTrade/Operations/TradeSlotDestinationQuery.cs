using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Creatures.Players;

namespace NeoServer.Game.Systems.SafeTrade.Operations;

internal static class TradeSlotDestinationQuery
{
    public static Slot Get(IPlayer player, IItem itemToAdd, IItem itemToBeRemoved)
    {
        if (itemToAdd.Metadata.BodyPosition is Slot.None) return Slot.Backpack;
        var existingItemOnSlot = player.Inventory[itemToAdd.Metadata.BodyPosition];

        if (itemToAdd.IsCumulative &&
            existingItemOnSlot.ServerId == itemToAdd.ServerId &&
            itemToAdd.Amount + existingItemOnSlot.Amount == 100) return itemToAdd.Metadata.BodyPosition;
        
        return existingItemOnSlot is null || existingItemOnSlot == itemToBeRemoved ? itemToAdd.Metadata.BodyPosition : Slot.Backpack;
    }
}