using Mediator;
using NeoServer.Application.Features.Shared;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Services;

namespace NeoServer.Application.Features.UseItem.UseOnItem;

public record UseItemOnItemCommand(IPlayer Player, IItem Item, IItem Target) : ICommand;

public class UseItemOnItemCommandHandler : ICommandHandler<UseItemOnItemCommand>
{
    private readonly IMap _map;
    private readonly WalkToTarget _walkToTarget;

    public UseItemOnItemCommandHandler(WalkToTarget walkToTarget, IMap map)
    {
        _walkToTarget = walkToTarget;
        _map = map;
    }

    public ValueTask<Unit> Handle(UseItemOnItemCommand command, CancellationToken cancellationToken)
    {
        command.Deconstruct(out var player, out var item, out var target);
        Guard.ThrowIfAnyNull(player, item, target);

        if (!player.IsNextTo(item)) return _walkToTarget.Go(player, item, () => Handle(command, cancellationToken));

        if (item.Location.Type is LocationType.Ground &&
            !item.IsNextTo(target) &&
            item.Metadata.OnUse is not null &&
            item.Metadata.OnUse.TryGetAttribute<bool>("pickfromground", out var pickFromGround) &&
            pickFromGround)
        {
            if (_map[item.Location] is not IDynamicTile dynamicTile)
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
            return _walkToTarget.Go(player, target, () => player.Use(item as IUsableOn, target));

        player.Use(item as IUsableOn, target);

        return Unit.ValueTask;
    }
}