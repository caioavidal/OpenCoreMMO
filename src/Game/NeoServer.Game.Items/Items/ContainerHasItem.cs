using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Bases;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;

namespace NeoServer.Game.Items.Items;

//todo: this class is a workaround. need to fix this
public class ContainerHasItem : HasItem
{
    private readonly IContainer container;

    public ContainerHasItem(IContainer container)
    {
        this.container = container;
    }

    public override Result<OperationResult<IItem>> AddItem(IItem thing, byte? position = null)
    {
        return container.AddItem(thing, position);
    }

    public override Result CanAddItem(IItem item, byte amount = 1, byte? slot = null)
    {
        return container.CanAddItem(item, amount, slot);
    }

    public override Result<uint> CanAddItem(IItemType itemType)
    {
        return container.CanAddItem(itemType);
    }

    public override bool CanRemoveItem(IItem item)
    {
        return container.CanRemoveItem(item);
    }

    public override uint PossibleAmountToAdd(IItem thing, byte? toPosition = null)
    {
        return container.PossibleAmountToAdd(thing, toPosition);
    }

    public override Result<OperationResult<IItem>> ReceiveFrom(IHasItem source, IItem thing, byte? toPosition)
    {
        return base.ReceiveFrom(source, thing, toPosition);
    }

    public override Result<OperationResult<IItem>> RemoveItem(IItem thing, byte amount, byte fromPosition,
        out IItem removedThing)
    {
        return container.RemoveItem(thing, amount, fromPosition, out removedThing);
    }

    public override Result<OperationResult<IItem>> SendTo(IHasItem destination, IItem thing, byte amount,
        byte fromPosition, byte? toPosition)
    {
        return base.SendTo(destination, thing, amount, fromPosition, toPosition);
    }
}