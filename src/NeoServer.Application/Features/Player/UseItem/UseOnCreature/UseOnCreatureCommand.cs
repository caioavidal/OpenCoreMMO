using Mediator;
using NeoServer.Application.Common;
using NeoServer.Application.Features.Shared;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Results;
using NeoServer.Game.Common.Services;
using NeoServer.Game.Common.Texts;

namespace NeoServer.Application.Features.Player.UseItem.UseOnCreature;

public record UseItemOnCreatureCommand(IPlayer Player, IUsableOn Item, ICreature Creature) : ICommand;

public class UseItemOnCreatureCommandHandler : ICommandHandler<UseItemOnCreatureCommand>
{
    private readonly IMapTool _mapTool;
    private readonly IMediator _mediator;
    private readonly WalkToTarget _walkToTarget;

    public UseItemOnCreatureCommandHandler(IMapTool mapTool, WalkToTarget walkToTarget, IMediator mediator)
    {
        _mapTool = mapTool;
        _walkToTarget = walkToTarget;
        _mediator = mediator;
    }

    public ValueTask<Unit> Handle(UseItemOnCreatureCommand command, CancellationToken cancellationToken)
    {
        Guard.ThrowIfAnyNull(command, command.Player, command.Creature);

        if (Guard.IsNull(command.Item))
        {
            OperationFailService.Send(command.Player, InvalidOperation.NotPossible);
            return Unit.ValueTask;
        }

        command.Deconstruct(out var player, out var item, out var target);

        if (!player.IsNextTo(item))
        {
            var operationResult = _walkToTarget.Go(player, item, () => _ = Handle(command, cancellationToken));
            if (operationResult.Failed)
            {
                OperationFailService.Send(player, operationResult.Error);
            }
            return Unit.ValueTask;
        }

        if (item.Metadata.OnUse is not null &&
            item.Metadata.OnUse.TryGetAttribute<bool>("walktotarget", out var walkToTarget) &&
            walkToTarget && !player.IsNextTo(target))
        {
            var operationResult = _walkToTarget.Go(player, target, () => _ = Handle(command, cancellationToken));
            if (operationResult.Failed)
            {
                OperationFailService.Send(player, operationResult.Error);
            }
            
            return Unit.ValueTask;
        }
            

        var canUseItem = CanUseItem(player, item, target.Location);
        if (canUseItem.Failed) OperationFailService.Send(player, canUseItem.Error);

        var result = player.Use(item, target);
        OperationFailService.Send(player, result.Error);

        _mediator.PublishGameEvents(player, target);

        return Unit.ValueTask;
    }

    private Result CanUseItem(IPlayer player, IUsableOn item, Location onLocation)
    {
        if (_mapTool.SightClearChecker?.Invoke(player.Location, onLocation, true) == false)
        {
            OperationFailService.Send(player.CreatureId, TextConstants.CANNOT_THROW_THERE);
            {
                return Result.Fail(InvalidOperation.CannotThrowThere);
            }
        }

        if (!item.IsCloseTo(player)) return Result.Fail(InvalidOperation.TooFar);

        if (item is IEquipmentRequirement requirement && !requirement.CanBeUsed(player))
        {
            OperationFailService.Send(player.CreatureId, requirement.ValidationError);
            {
                return Result.Fail(InvalidOperation.CannotUse);
            }
        }

        return Result.Success;
    }
}