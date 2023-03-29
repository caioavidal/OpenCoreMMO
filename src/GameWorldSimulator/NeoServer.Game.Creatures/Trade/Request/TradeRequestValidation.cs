using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Services;

namespace NeoServer.Game.Creatures.Trade.Request;

internal static class TradeRequestValidation
{
    public static bool IsValid(IPlayer player, IPlayer secondPlayer, IItem item)
    {
        if (player is not Player.Player playerRequesting || secondPlayer is not Player.Player playerRequested)
        {
            return false;
        }
        
        // Ensure that the two players are not the same
        if (playerRequesting == playerRequested)
        {
            OperationFailService.Send(player.CreatureId, "Select a player to trade with.");
            return false;
        }

        // Ensure that the player requesting the trade is not already in a trade
        if (item == playerRequested.LastTradeRequest?.Item)
        {
            OperationFailService.Send(player.CreatureId, "This item is already being traded.");
            return false;
        }
        
        // Ensure that the player requesting the trade is not already in a trade
        if (playerRequesting.LastTradeRequest?.Item is { })
        {
            OperationFailService.Send(player.CreatureId, "You are already trading.");
            return false;    
        }
        
        // Ensure that the player is close enough to the item being traded
        if (!player.Location.IsNextTo(item.Location))
        {
            OperationFailService.Send(player.CreatureId, $"You are not close to item.");
            return false;
        }

        // Ensure that both players are close enough to each other
        if (!player.Location.IsNextTo(secondPlayer.Location))
        {
            OperationFailService.Send(player.CreatureId, $"{secondPlayer.Name} tells you to move close.");
            return false;
        }
        
        // Ensure that the player being traded with is not already in a trade with someone else
        if (playerRequested.LastTradeRequest is not null &&
            playerRequested.LastTradeRequest?.PlayerRequested.CreatureId != player.CreatureId)
        {
            OperationFailService.Send(player.CreatureId, "Player is already trading.");
            return false;
        }

        return true;
    }
}