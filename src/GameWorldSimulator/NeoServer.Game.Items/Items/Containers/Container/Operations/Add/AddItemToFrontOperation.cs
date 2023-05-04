using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Results;
using NeoServer.Game.Items.Items.Containers.Container.Operations.Update;

namespace NeoServer.Game.Items.Items.Containers.Container.Operations.Add;

internal static class AddItemToFrontOperation
{
    public static Result Add(Container toContainer, IItem item)
    {
        if (item is null) return Result.NotPossible;
        if (toContainer.SlotsUsed >= toContainer.Capacity) return new Result(InvalidOperation.IsFull);
        item.SetNewLocation(toContainer.Location);
        toContainer.Items.Insert(0, item);
        toContainer.SlotsUsed++;

        if (item is IContainer container) container.SetParent(toContainer);

        ItemsLocationOperation.Update(toContainer);

        if (item is ICumulative cumulative) cumulative.OnReduced += toContainer.OnItemReduced;

        toContainer.InvokeItemAddedEvent(item, toContainer);
        return Result.Success;
    }
}