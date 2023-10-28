using Mediator;
using NeoServer.Application.Features.Shared;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Helpers;

namespace NeoServer.Application.Features.Player.UseItem.UseItem;

public record OpenContainerCommand(IPlayer Player, IContainer Container, byte OpenAtIndex) : ICommand;

public class OpenContainerCommandHandler : ICommandHandler<OpenContainerCommand>
{
    private readonly WalkToTarget _walkToTarget;

    public OpenContainerCommandHandler(WalkToTarget walkToTarget)
    {
        _walkToTarget = walkToTarget;
    }

    public ValueTask<Unit> Handle(OpenContainerCommand command, CancellationToken cancellationToken)
    {
        command.Deconstruct(out var player, out var container, out var openAtIndex);

        Guard.ThrowIfAnyNull(player, container);

        if (!player.IsNextTo(container))
            return _walkToTarget.Go(player, container, () => Handle(command, cancellationToken));

        player.Use(container, openAtIndex);

        return Unit.ValueTask;
    }
}