using System.Linq;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Services;

namespace NeoServer.Game.Creatures.Trade.Validations;

internal static class TradeRequestValidation
{
    public static bool IsValid(IPlayer player, IPlayer secondPlayer, IItem[] items)
    {
        if (player is not Player.Player playerRequesting || secondPlayer is not Player.Player playerRequested) return false;

        // Ensure that the two players are not the same
        if (BothPlayersAreTheSame(player, playerRequesting, playerRequested)) return false;

        if (TradeHasNoItems(player, items)) return false;
        
        if (items.Length > 255)
        {
            OperationFailService.Send(player.CreatureId, "You cannot trade more than 255 items at once.");
            return false;
        }

        if (ItemIsAlreadyBeingTraded(player, items, playerRequested)) return false;
        
        // Ensure that the player is close enough to the item being traded
        if (!PlayerIsNextToItem(player, items)) return false;

        // Ensure that the player requesting the trade is not already in a trade
        if (playerRequesting.CurrentTradeRequest?.Items is { })
        {
            OperationFailService.Send(player.CreatureId, "You are already trading.");
            return false;
        }

        // Ensure that both players are close enough to each other
        if (!player.Location.IsNextTo(secondPlayer.Location))
        {
            OperationFailService.Send(player.CreatureId, $"{secondPlayer.Name} tells you to move close.");
            return false;
        }

        // Ensure that the second player is not already in a trade with someone else
        if (SecondPlayerIsAlreadyTrading(player, playerRequested)) return false;

        return true;
    }

    private static bool BothPlayersAreTheSame(IPlayer player, Player.Player playerRequesting, Player.Player playerRequested)
    {
        if (playerRequesting == playerRequested)
        {
            OperationFailService.Send(player.CreatureId, "Select a player to trade with.");
            return true;
        }

        return false;
    }

    private static bool SecondPlayerIsAlreadyTrading(IPlayer player, Player.Player playerRequested)
    {
        if (playerRequested.CurrentTradeRequest is not null &&
            playerRequested.CurrentTradeRequest?.PlayerRequested.CreatureId != player.CreatureId)
        {
            OperationFailService.Send(player.CreatureId, "Player is already trading.");
            return true;
        }

        return false;
    }

    private static bool PlayerIsNextToItem(IPlayer player, IItem[] items)
    {
        if (!player.Location.IsNextTo(items[0].Location))
        {
            OperationFailService.Send(player.CreatureId, "You are not close to the item.");
            return false;
        }

        return true;
    }

    private static bool TradeHasNoItems(IPlayer player, IItem[] items)
    {
        if (items is null || !items.Any())
        {
            OperationFailService.Send(player.CreatureId, "You must select an item to trade.");
            return true;
        }

        return false;
    }

    private static bool ItemIsAlreadyBeingTraded(IPlayer player, IItem[] items, Player.Player playerRequested)
    {
        foreach (var item in items)
        {
            if (ItemTradedTracker.GetTradeRequest(item) is not null ||
                (playerRequested.CurrentTradeRequest?.Items.Contains(item) ?? false))
            {
                OperationFailService.Send(player.CreatureId, "This item is already being traded.");
                return true;
            }
        }

        return false;
    }
}