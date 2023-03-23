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

        return true;
    }
}