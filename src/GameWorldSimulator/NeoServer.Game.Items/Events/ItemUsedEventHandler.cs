using NeoServer.Game.Common;
using NeoServer.Game.Common.Chats;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Results;

namespace NeoServer.Game.Items.Events;

public class ItemUsedEventHandler : IGameEventHandler
{
    private readonly IItemFactory itemFactory;
    private readonly IMap map;

    public ItemUsedEventHandler(IMap map, IItemFactory itemFactory)
    {
        this.map = map;
        this.itemFactory = itemFactory;
    }

    public void Execute(ICreature usedBy, ICreature creature, IItem item)
    {
        Transform(usedBy, creature, item);
        Say(creature, item);
    }

    private void Transform(ICreature usedBy, ICreature creature, IItem item)
    {
        if (item?.CanTransformTo == 0) return;
        if (usedBy is not IPlayer player) return;
        var createdItem = itemFactory.Create(item.CanTransformTo, creature.Location, null);

        if (map[creature.Location] is not IDynamicTile tile) return;

        if (item?.Location.Type == LocationType.Ground) tile.AddItem(createdItem);
        if (item?.Location.Type == LocationType.Container)
        {
            var container = player.Containers[item.Location.ContainerId] ?? player.Inventory?.BackpackSlot;

            var result = container?.AddItem(createdItem) ??
                         new Result<OperationResultList<IItem>>(InvalidOperation.NotPossible);
            if (!result.Succeeded) tile.AddItem(createdItem);
        }
    }

    private void Say(ICreature creature, IItem item)
    {
        if (item is IConsumable consumable && !string.IsNullOrWhiteSpace(consumable.Sentence))
            creature.Say(consumable.Sentence, SpeechType.MonsterSay);
    }
}