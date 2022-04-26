using System;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;

namespace NeoServer.Game.Common.Contracts.Bases;

public abstract class HasItem : IHasItem
{
    public abstract Result<OperationResult<IItem>> AddItem(IItem thing, byte? position = null);
    public abstract Result CanAddItem(IItem item, byte amount = 1, byte? slot = null);
    public abstract Result<uint> CanAddItem(IItemType itemType);

    public abstract bool CanRemoveItem(IItem item);

    public abstract uint PossibleAmountToAdd(IItem thing, byte? toPosition = null);

    public virtual Result<OperationResult<IItem>> ReceiveFrom(IHasItem source, IItem thing, byte? toPosition)
    {
        var canAdd = CanAddItem(thing, thing.Amount, toPosition);
        if (!canAdd.Succeeded) return new Result<OperationResult<IItem>>(canAdd.Error);

        var result = AddItem(thing, toPosition);
        
        return result;
    }

    public abstract Result<OperationResult<IItem>> RemoveItem(IItem thing, byte amount, byte fromPosition,
        out IItem removedThing);

  
}