using Mediator;
using NeoServer.Application.Features.Decay;
using NeoServer.Application.Features.Shared;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Results;
using NeoServer.Game.Common.Services;

namespace NeoServer.Application.Features.Player;

public record DressEquipmentCommand
    (IPlayer Player, IItem Equipment, IHasItem From, byte FromPosition, byte Amount, Slot Slot) : ICommand;

public class DressEquipmentCommandHandler : ICommandHandler<DressEquipmentCommand>
{
    private readonly IItemDecayTracker _decayTracker;
    private readonly WalkToTarget _walkToTarget;

    public DressEquipmentCommandHandler(WalkToTarget walkToTarget, IItemDecayTracker decayTracker)
    {
        _walkToTarget = walkToTarget;
        _decayTracker = decayTracker;
    }

    public ValueTask<Unit> Handle(DressEquipmentCommand command, CancellationToken cancellationToken)
    {
        var error = Move(command, command.Amount);

        if (error is not InvalidOperation.None)
        {
            OperationFailService.Send(command.Player, error);
            return Unit.ValueTask;
        }

        _decayTracker.Track(command.Equipment);

        return Unit.ValueTask;
    }

    private InvalidOperation Move(DressEquipmentCommand command, byte amount)
    {
        Guard.ThrowIfAnyNull(command, command.Player, command.Equipment, command.From);

        command.Deconstruct(out var player, out var item, out var from, out var fromPosition,
            out _, out var slot);

        var destination = player.Inventory;

        if (item is null) return InvalidOperation.None;

        if (!item.CanBeMoved) return InvalidOperation.CannotMove;

        if (!player.IsNextTo(item))
        {
            _ = _walkToTarget.Go(player, item, () => Move(command, command.Amount));
            return InvalidOperation.None;
        }

        var canAdd = destination.CanAddItem(item, amount, (byte)slot);

        if (!canAdd.Succeeded) return canAdd.Error;

        var possibleAmountToAdd = destination.PossibleAmountToAdd(item, (byte)slot);
        if (possibleAmountToAdd == 0) return InvalidOperation.NotEnoughRoom;

        var removedItem = RemoveItem(item, from, amount, fromPosition, possibleAmountToAdd);

        var operationResult = AddToDestination(removedItem, from, destination, slot);
        if (operationResult.Failed) return operationResult.Error;

        var amountResult = (byte)Math.Max(0, amount - (int)possibleAmountToAdd);

        if (amountResult > 0) return Move(command, amountResult);

        return InvalidOperation.None;
    }

    private static Result<OperationResultList<IItem>> AddToDestination(IItem thing, IHasItem source,
        IHasItem destination, Slot slot)
    {
        var slotIndex = (byte)slot;
        var canAdd = destination.CanAddItem(thing, thing.Amount, slotIndex);
        if (!canAdd.Succeeded) return new Result<OperationResultList<IItem>>(canAdd.Error);

        var result = destination.AddItem(thing, slotIndex);

        if (!result.Value.HasAnyOperation) return result;

        foreach (var operation in result.Value.Operations)
            if (operation.Item2 == Operation.Removed)
                source.AddItem(operation.Item1);

        return result;
    }

    private static IItem RemoveItem(IItem item, IHasItem from, byte amount, byte fromPosition, uint possibleAmountToAdd)
    {
        var amountToRemove = item is not ICumulative ? (byte)1 : (byte)Math.Min(amount, possibleAmountToAdd);

        from.RemoveItem(item, amountToRemove, fromPosition, out var removedThing);

        return removedThing;
    }
}