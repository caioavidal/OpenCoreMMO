using System;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Results;
using NeoServer.Game.Items.Items.Containers.Container.Operations.Update;
using NeoServer.Game.Items.Items.Containers.Container.Queries;

namespace NeoServer.Game.Items.Items.Containers.Container.Operations.Add;

internal static class AddItemOperation
{
    public static Result Add(Container toContainer, IItem item, byte slot)
    {
        if (item is null) return Result.NotPossible;
        if (toContainer.Capacity <= slot) throw new ArgumentOutOfRangeException("Slot is greater than capacity");

        if (item is not ICumulative cumulativeItem) return AddItemToFrontOperation.Add(toContainer, item);

        var itemToJoinSlot = FindSlotOfFirstItemNotFullyQuery.Find(onContainer: toContainer, cumulativeItem);

        if (itemToJoinSlot >= 0 && cumulativeItem is { } cumulative)
            return JoinCumulativeItemOperation.Join(toContainer, cumulative, (byte)itemToJoinSlot);
        
        return AddItemToFrontOperation.Add(toContainer, cumulativeItem);
    }
}