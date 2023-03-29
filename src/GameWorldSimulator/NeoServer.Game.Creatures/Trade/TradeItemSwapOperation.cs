using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Results;
using NeoServer.Game.Common.Services;
using NeoServer.Game.Creatures.Services;
using NeoServer.Game.Creatures.Trade.Request;

namespace NeoServer.Game.Creatures.Trade;

public class TradeItemSwapOperation
{
    private readonly IMap _map;

    public TradeItemSwapOperation(IMap map)
    {
        _map = map;
    }
    
    private const string DEFAULT_ERROR_MESSAGE = "Trade could not be completed.";

    public bool Swap(TradeRequest tradeRequest)
    {
        var playerRequesting = tradeRequest.PlayerRequesting;
        var playerRequested = tradeRequest.PlayerRequested;

        var itemFromPlayerRequesting = tradeRequest.PlayerRequesting.LastTradeRequest.Item;
        var itemFromPlayerRequested = tradeRequest.PlayerRequested.LastTradeRequest.Item;

        if (!CanAddItemToPlayers(playerRequested, itemFromPlayerRequesting, playerRequesting, itemFromPlayerRequested)) return false;

        Move(itemFromPlayerRequested, playerRequesting);
        Move(itemFromPlayerRequesting, playerRequested);
        
        return true;
    }

    private static bool CanAddItemToPlayers(IPlayer playerRequested, IItem itemFromPlayerRequesting, IPlayer playerRequesting,
        IItem itemFromPlayerRequested)
    {
        if (!CanAddItem(playerRequested, itemFromPlayerRequesting))
        {
            OperationFailService.Send(playerRequesting.CreatureId, DEFAULT_ERROR_MESSAGE);
            return false;
        }

        if (CanAddItem(playerRequesting, itemFromPlayerRequested)) return true;
        
        OperationFailService.Send(playerRequested.CreatureId, DEFAULT_ERROR_MESSAGE);
        return false;

    }

    private static bool CanAddItem(IPlayer player, IItem item)
    {
        var slot = GetSlotDestination(player, item);
        var result = player.Inventory.CanAddItem(item, item.Amount, (byte)slot);

        if (result.Succeeded)
        {
            return true;
        }

        switch (result.Error)
        {
            case InvalidOperation.NotEnoughRoom:
                OperationFailService.Send(player.CreatureId, "You do not have enough room to carry this object.");
                break;
            case InvalidOperation.TooHeavy:
                OperationFailService.Send(player.CreatureId, $"You do not have enough capacity to carry this object.\nIt weighs {item.Weight} oz.");
                break;
            default:
                OperationFailService.Send(player.CreatureId, DEFAULT_ERROR_MESSAGE);
                break;
        }

        return false;
    }

    private void Move(IItem item, IPlayer destination)
    {
        if (Guard.AnyNull(item)) return;

        if (item.Location.Type is LocationType.Ground)
        {
            var tile = _map[item.Location];
            if (tile is not IDynamicTile fromTile) return;

            fromTile.RemoveItem(item);
        }

        if (item.Parent is IContainer container) container.RemoveItem(item, item.Amount);

        AddItemToInventory(destination, item);
    }

    private static void AddItemToInventory(IPlayer player, IItem item)
    {
        var slot = GetSlotDestination(player, item);
        player.Inventory.AddItem(item, slot);
    }

    private static Slot GetSlotDestination(IPlayer player, IItem item)
    {
        if (item.Metadata.BodyPosition is Slot.None) return Slot.Backpack;
        return player.Inventory[item.Metadata.BodyPosition] is null ? item.Metadata.BodyPosition : Slot.Backpack;
    }
}