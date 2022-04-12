using System;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Services;

namespace NeoServer.Game.Creatures.Services;

public class MoveService:IMoveService
{
    public Result<OperationResult<IItem>> Move(IItem item, IHasItem from, IHasItem destination, byte amount, byte fromPosition, byte? toPosition)
    {
        var canAdd = destination.CanAddItem(item, amount, toPosition);
        if (!canAdd.IsSuccess) return new Result<OperationResult<IItem>>(canAdd.Error);

        var possibleAmountToAdd = destination.PossibleAmountToAdd(item, toPosition);
        if (possibleAmountToAdd == 0) return new Result<OperationResult<IItem>>(InvalidOperation.NotEnoughRoom);

        IItem removedThing;
        if (item is not ICumulative)
        {
            from.RemoveItem(item, 1, fromPosition, out removedThing);
        }
        else
        {
            var amountToAdd = (byte)Math.Min(amount, possibleAmountToAdd);

            from.RemoveItem(item, amountToAdd, fromPosition, out removedThing);
        }

        var result = destination.ReceiveFrom(from, removedThing, toPosition);

        if (result.IsSuccess && item is IMovableThing movableThing && destination is IThing destinationThing) movableThing.OnMoved(destinationThing);
        
        var amountResult = (byte)Math.Max(0, amount - (int)possibleAmountToAdd);
        return amountResult > 0 ? Move(item, @from, destination, amountResult, fromPosition, toPosition) : result;
    }
}