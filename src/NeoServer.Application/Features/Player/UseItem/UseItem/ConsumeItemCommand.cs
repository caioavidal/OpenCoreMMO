using Mediator;
using NeoServer.Application.Features.Shared;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Results;
using NeoServer.Game.Common.Services;

namespace NeoServer.Application.Features.Player.UseItem.UseItem;

public sealed record ConsumeItemCommand(IPlayer Player, IConsumable Item, IThing Destination) : ICommand;

public class ConsumeItemCommandHandler(IItemFactory itemFactory, IMap map, WalkToTarget walkToTarget)
    : ICommandHandler<ConsumeItemCommand>
{
    public ValueTask<Unit> Handle(ConsumeItemCommand command, CancellationToken cancellationToken)
    {
        var player = command.Player;
        var destination = command.Destination ?? command.Player;
        var item = command.Item;

        if (!player.IsNextTo(item))
        {
            var operationResult = walkToTarget.Go(command.Player, command.Item, () => Handle(command, cancellationToken));
            if (operationResult.Failed)
            {
                OperationFailService.Send(player, operationResult.Error);
            }
            return Unit.ValueTask;
        }

        item.Use(player, destination as ICreature);

        Transform(player, destination as ICreature, item);
        Say(destination as ICreature, item);

        return Unit.ValueTask;
    }

    private void Transform(ICreature usedBy, ICreature creature, IItem item)
    {
        if (Guard.AnyNull(usedBy, creature, item)) return;

        if (item is { CanTransformTo: 0 }) return;
        if (usedBy is not IPlayer player) return;

        var createdItem = itemFactory.Create(item.CanTransformTo, creature.Location, null);

        if (map[creature.Location] is not IDynamicTile tile) return;

        if (item.Location.Type is LocationType.Ground) tile.AddItem(createdItem);

        if (item.Location.Type is LocationType.Container)
        {
            var container = player.Containers[item.Location.ContainerId] ?? player.Inventory?.BackpackSlot;

            var result = container?.AddItem(createdItem) ??
                         new Result<OperationResultList<IItem>>(InvalidOperation.NotPossible);
            if (!result.Succeeded) tile.AddItem(createdItem);
        }
    }

    private static void Say(ICreature creature, IConsumable item)
    {
        if (string.IsNullOrWhiteSpace(item.Sentence)) return;
        creature.Say(item.Sentence, SpeechType.MonsterSay);
    }
}