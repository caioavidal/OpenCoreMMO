using System;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Results;

namespace NeoServer.Game.Items.Items.Containers.Container.Operations;

internal static class RemoveFromContainerOperation
{
    public static OperationResult<IItem> RemoveItem(Container fromContainer, byte slotIndex, byte amount)
    {
        var item = fromContainer.Items[slotIndex];

        var amountToReduce = Math.Min(item.Amount, amount);

        if (item is ICumulative cumulative && amountToReduce != item.Amount) return ReduceItem(cumulative, amount);

        fromContainer.Items.RemoveAt(slotIndex);

        RemoveParentIfContainer(item);

        UnsubscribeIfCumulativeItem(fromContainer, item);

        return OperationResult<IItem>.Ok(item, Operation.Removed);
    }

    private static OperationResult<IItem> ReduceItem(ICumulative cumulative, byte amount)
    {
        var itemSplit = cumulative.Split(amount);

        return OperationResult<IItem>.Ok(itemSplit, Operation.Updated);
    }

    private static void UnsubscribeIfCumulativeItem(Container fromContainer, IItem item)
    {
        if (item is not ICumulative cumulative) return;
        cumulative.OnReduced -= fromContainer.OnItemReduced;
    }

    private static void RemoveParentIfContainer(IItem item)
    {
        if (item is not IContainer container) return;
        container.SetParent(null);
    }
}