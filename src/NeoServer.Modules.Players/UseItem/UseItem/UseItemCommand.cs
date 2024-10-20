using Mediator;
using NeoServer.BuildingBlocks.Domain;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Services;

namespace NeoServer.Modules.Players.UseItem.UseItem;

public sealed record UseItemCommand(IPlayer Player, IItem Item) : ICommand;

public sealed class UseItemCommandHandler(IWalkToTarget walkToTarget) : ICommandHandler<UseItemCommand>
{
    public ValueTask<Unit> Handle(UseItemCommand command, CancellationToken cancellationToken)
    {
        command.Deconstruct(out var player, out var item);

        Guard.ThrowIfAnyNull(player, item);

        if (!player.IsNextTo(item))
        {
            var operationResult = walkToTarget.Go(player, item, () => Handle(command, cancellationToken));
            if (operationResult.Failed)
            {
                OperationFailService.Send(player, operationResult.Error);
            }
            return Unit.ValueTask;
        }

        player.Use(item);

        return Unit.ValueTask;
    }
}