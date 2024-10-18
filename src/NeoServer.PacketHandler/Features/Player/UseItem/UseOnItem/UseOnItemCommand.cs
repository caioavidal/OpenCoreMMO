using Mediator;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Results;
using NeoServer.Game.Common.Services;
using NeoServer.PacketHandler.Features.Shared;

namespace NeoServer.PacketHandler.Features.Player.UseItem.UseOnItem;

public record UseItemOnItemCommand(IPlayer Player, IItem Item, IItem Target) : ICommand;

public class UseItemOnItemCommandHandler(WalkToTarget toTarget, IMap map) : ICommandHandler<UseItemOnItemCommand>
{
    public ValueTask<Unit> Handle(UseItemOnItemCommand command, CancellationToken cancellationToken)
    {
        command.Deconstruct(out var player, out var item, out var target);
        Guard.ThrowIfAnyNull(player, target);

        if (Guard.IsNull(item))
        {
            OperationFailService.Send(command.Player, InvalidOperation.NotPossible);
            return Unit.ValueTask;
        }

        if (!player.IsNextTo(item))
        {
            var operationResult = toTarget.Go(player, item, () => Handle(command, cancellationToken));
            if (operationResult.Failed)
            {
                OperationFailService.Send(player, operationResult.Error);
            }

            return Unit.ValueTask;
        }

        if (item.Location.Type is LocationType.Ground &&
            !item.IsNextTo(target) &&
            item.Metadata.OnUse is not null &&
            item.Metadata.OnUse.TryGetAttribute<bool>("pickfromground", out var pickFromGround) &&
            pickFromGround)
        {
            if (map[item.Location] is not IDynamicTile dynamicTile)
            {
                OperationFailService.Send(player, InvalidOperation.NotPossible);
                return Unit.ValueTask;
            }

            var result = player.PickItemFromGround(item, dynamicTile);
            if (result.Failed) OperationFailService.Send(player, result.Error);
        }

        if (!player.IsNextTo(target) &&
            item.Metadata.OnUse is not null &&
            item.Metadata.OnUse.TryGetAttribute<bool>("walktotarget", out var walkToTarget) &&
            walkToTarget)
        {
            var operationResult = toTarget.Go(player, target, () => _ = Handle(command, cancellationToken).Result);

            if (operationResult.Failed)
            {
                OperationFailService.Send(player, operationResult.Error);
            }

            return Unit.ValueTask;
        }

        var useResult = Use(player, item, target);

        if (useResult.Failed)
        {
            OperationFailService.Send(player, useResult.Error,
                useResult.Error is InvalidOperation.CanOnlyUseRuneOnCreature ? EffectT.Puff : EffectT.None);
        }

        return Unit.ValueTask;
    }

    private Result Use(IPlayer player, IItem item, IItem target)
    {
        if (target.Location.Type is LocationType.Ground && map[target.Location] is { } tile)
            return player.Use(item as IUsableOn, tile);

        return player.Use(item as IUsableOn, target);
    }
}