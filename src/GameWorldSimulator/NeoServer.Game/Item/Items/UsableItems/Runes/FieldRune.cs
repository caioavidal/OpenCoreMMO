using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Runes;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Results;
using NeoServer.Game.Item.Items.UsableItems.Runes.Events;

namespace NeoServer.Game.Item.Items.UsableItems.Runes;

public class FieldRune : Rune, IFieldRune
{
    public FieldRune(IItemType type, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(
        type, location, attributes)
    {
    }

    public FieldRune(IItemType type, Location location, byte amount) : base(type, location, amount)
    {
    }

    public override ushort Duration => 2;
    public event UseOnTile OnUsedOnTile;
    public ushort Field => Metadata.Attributes.GetAttribute<ushort>(ItemAttribute.Field);

    public virtual string Area => Metadata.Attributes.GetAttribute(ItemAttribute.Area);

    public Result Use(ICreature usedBy, ITile tile)
    {
        if (tile is not IDynamicTile dynamicTile) return Result.Fail(InvalidOperation.NotEnoughRoom);
        OnUsedOnTile?.Invoke(usedBy, dynamicTile, this);

        RaiseEvent(new FieldRuneUsedOnTileEvent(usedBy as IPlayer, this, dynamicTile));

        Reduce();

        return Result.Success;
    }

    public Queue<IGameEvent> Events { get; } = new();

    public void RaiseEvent(IGameEvent gameEvent)
    {
        Events.Enqueue(gameEvent);
    }

    public new static bool IsApplicable(IItemType type)
    {
        return type.Group is ItemGroup.FieldRune;
    }
}