using System;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;

namespace NeoServer.Game.Common.Contracts.Bases;

public abstract class Store : IStore
{
    public abstract Result<OperationResult<IItem>> AddItem(IItem thing, byte? position = null);
    public abstract Result CanAddItem(IItem item, byte amount = 1, byte? slot = null);
    public abstract Result<uint> CanAddItem(IItemType itemType);

    public abstract bool CanRemoveItem(IItem item);

    public abstract uint PossibleAmountToAdd(IItem thing, byte? toPosition = null);

    public virtual Result<OperationResult<IItem>> ReceiveFrom(IStore source, IItem thing, byte? toPosition)
    {
        var canAdd = CanAddItem(thing, thing.Amount, toPosition);
        if (!canAdd.IsSuccess) return new Result<OperationResult<IItem>>(canAdd.Error);

        var result = AddItem(thing, toPosition);

        return result;
    }

    public abstract Result<OperationResult<IItem>> RemoveItem(IItem thing, byte amount, byte fromPosition,
        out IItem removedThing);

    public virtual Result<OperationResult<IItem>> SendTo(IStore destination, IItem thing, byte amount,
        byte fromPosition, byte? toPosition)
    {
        var canAdd = destination.CanAddItem(thing, amount, toPosition);
        if (!canAdd.IsSuccess) return new Result<OperationResult<IItem>>(canAdd.Error);

        var possibleAmountToAdd = destination.PossibleAmountToAdd(thing, toPosition);
        if (possibleAmountToAdd == 0) return new Result<OperationResult<IItem>>(InvalidOperation.NotEnoughRoom);

        IItem removedThing;
        if (thing is not ICumulative cumulative)
        {
            if (possibleAmountToAdd < 1) return new Result<OperationResult<IItem>>(InvalidOperation.NotEnoughRoom);
            RemoveItem(thing, 1, fromPosition, out removedThing);
        }
        else
        {
            var amountToAdd = (byte)Math.Min(amount, possibleAmountToAdd);

            RemoveItem(thing, amountToAdd, fromPosition, out removedThing);
        }

        var result = destination.ReceiveFrom(this, removedThing, toPosition);

        if (result.IsSuccess && thing is IMoveableThing moveableThing) moveableThing.OnMoved();

        var amountResult = (byte)Math.Max(0, amount - (int)possibleAmountToAdd);
        if (amountResult > 0) return SendTo(destination, thing, amountResult, fromPosition, toPosition);

        return result;
    }
}