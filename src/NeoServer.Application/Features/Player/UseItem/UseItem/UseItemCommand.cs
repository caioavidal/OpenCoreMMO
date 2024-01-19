using Mediator;
using NeoServer.Application.Features.Shared;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Helpers;

namespace NeoServer.Application.Features.Player.UseItem.UseItem;

public sealed record UseItemCommand(IPlayer Player, IItem Item) : ICommand;

public sealed class UseItemCommandHandler : ICommandHandler<UseItemCommand>
{
    private readonly WalkToTarget _walkToTarget;

    public UseItemCommandHandler(WalkToTarget walkToTarget)
    {
        _walkToTarget = walkToTarget;
    }

    public ValueTask<Unit> Handle(UseItemCommand command, CancellationToken cancellationToken)
    {
        command.Deconstruct(out var player, out var item);

        Guard.ThrowIfAnyNull(player, item);

        if (!player.IsNextTo(item))
            return _walkToTarget.Go(player, item, () => Handle(command, cancellationToken));

        player.Use(item);

        return Unit.ValueTask;
    }
}