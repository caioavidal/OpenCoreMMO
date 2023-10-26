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

namespace NeoServer.Application.Features.UseItem.UseItem;

public sealed record ConsumeItemCommand(IPlayer Player, IConsumable Item, IThing Destination) : ICommand;

public class ConsumeItemCommandHandler : ICommandHandler<ConsumeItemCommand>
{
    private readonly IItemFactory _itemFactory;
    private readonly IMap _map;
    private readonly WalkToTarget _walkToTarget;

    public ConsumeItemCommandHandler(IItemFactory itemFactory, IMap map, WalkToTarget walkToTarget)
    {
        _itemFactory = itemFactory;
        _map = map;
        _walkToTarget = walkToTarget;
    }

    public ValueTask<Unit> Handle(ConsumeItemCommand request, CancellationToken cancellationToken)
    {
        var player = request.Player;
        var destination = request.Destination ?? request.Player;
        var item = request.Item;

        if (!player.IsNextTo(item))
            return _walkToTarget.Go(request.Player, request.Item, () => Handle(request, cancellationToken));

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

        var createdItem = _itemFactory.Create(item.CanTransformTo, creature.Location, null);

        if (_map[creature.Location] is not IDynamicTile tile) return;

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