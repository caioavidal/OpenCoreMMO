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
        
        if (playerRequesting == playerRequested)
        {
            OperationFailService.Send(player.CreatureId, "Select a player to trade with.");
            return false;
        }

        if (item == playerRequested.LastTradeRequest?.Item)
        {
            OperationFailService.Send(player.CreatureId, "This item is already being traded.");
            return false;
        }
        
        if (playerRequesting.LastTradeRequest?.Item is { })
        {
            OperationFailService.Send(player.CreatureId, "You are already trading.");
            return false;    
        }
        
        if (!player.Location.IsNextTo(item.Location))
        {
            OperationFailService.Send(player.CreatureId, $"You are not close to item.");
            return false;
        }

        if (!player.Location.IsNextTo(secondPlayer.Location))
        {
            OperationFailService.Send(player.CreatureId, $"{secondPlayer.Name} tells you to move close.");
            return false;
        }
        
        if (playerRequested.LastTradeRequest is not null &&
            playerRequested.LastTradeRequest?.PlayerRequested.CreatureId != player.CreatureId)
        {
            OperationFailService.Send(player.CreatureId, "Player is already trading.");
            return false;
        }

        return true;
    }
}