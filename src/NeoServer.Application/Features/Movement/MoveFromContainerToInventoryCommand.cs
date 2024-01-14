using Mediator;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Application.Features.Movement;

public sealed record MoveFromContainerToInventoryCommand(IPlayer Player, Location FromLocation, Location ToLocation,
    byte Amount) : ICommand;

public class MoveFromContainerToInventoryCommandHandler : ICommandHandler<MoveFromContainerToInventoryCommand>
{
    // public void Handle(IPlayer player, PlayerMoveItemCommand moveItemCommand)
    // {
    //     var container = player.Containers[moveItemCommand.FromLocation.ContainerId];
    //
    //     var item = container[moveItemCommand.FromLocation.ContainerSlot];
    //
    //     if (item is null) return;
    //
    //     if (!item.IsPickupable) return;
    //
    //     player.MoveItem(item, container, player.Inventory, moveItemCommand.Amount,
    //         (byte)moveItemCommand.FromLocation.ContainerSlot, (byte)moveItemCommand.ToLocation.Slot);
    // }
    public ValueTask<Unit> Handle(MoveFromContainerToInventoryCommand command, CancellationToken cancellationToken)
    {
        var container = command.Player.Containers[command.FromLocation.ContainerId];

        var item = container[command.FromLocation.ContainerSlot];

        if (item is null) return Unit.ValueTask;

        if (!item.IsPickupable) return Unit.ValueTask;

        command.Player.MoveItem(item, container, command.Player.Inventory, command.Amount,
            (byte)command.FromLocation.ContainerSlot, (byte)command.ToLocation.Slot);
        return Unit.ValueTask;
    }
}