using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Services;

namespace NeoServer.Game.Creatures.Trade.Request;

internal static class TradeRequestValidation
{
    public static bool IsValid(IPlayer player, IPlayer secondPlayer, IItem item)
    {
        if (player == secondPlayer)
        {
            OperationFailService.Send(player.CreatureId, "Select a player to trade with.");
            return false;
        }
        
        if (((Player.Player)player).LastTradeRequest?.Item is { })
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
        
        if (((Player.Player)secondPlayer).LastTradeRequest is not null &&
            ((Player.Player)secondPlayer).LastTradeRequest.PlayerRequested.CreatureId != player.CreatureId)
        {
            OperationFailService.Send(player.CreatureId, "Player is already trading.");
        }

        return true;
    }
}