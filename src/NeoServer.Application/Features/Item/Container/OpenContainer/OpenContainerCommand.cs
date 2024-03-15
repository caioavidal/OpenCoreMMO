using Mediator;
using NeoServer.Application.Features.Shared;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Services;

namespace NeoServer.Application.Features.Item.Container.OpenContainer;

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
        {
            var operationResult = _walkToTarget.Go(player, container, () => Handle(command, cancellationToken));

            if (operationResult.Failed)
            {
                OperationFailService.Send(player, operationResult.Error);
            }
            return Unit.ValueTask;
        }

        player.Use(container, openAtIndex);

        return Unit.ValueTask;
    }
}