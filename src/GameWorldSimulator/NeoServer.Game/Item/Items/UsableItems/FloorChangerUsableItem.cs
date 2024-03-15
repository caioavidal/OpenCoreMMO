using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Results;

namespace NeoServer.Game.Item.Items.UsableItems;

public class FloorChangerUsableItem : UsableOnItem, IUsableOnItem
{
    public FloorChangerUsableItem(IItemType type, Location location) : base(type, location)
    {
    }

    public override bool AllowUseOnDistance => false;

    public virtual Result Use(ICreature usedBy, IItem onItem)
    {
        if (usedBy is not IPlayer player) throw new ArgumentException("Invalid argument", nameof(usedBy));
        var canUseOnItems = Metadata.OnUse?.GetAttributeArray<ushort>(ItemAttribute.UseOn) ?? Array.Empty<ushort>();

        if (!canUseOnItems.Contains(onItem.Metadata.TypeId)) return Result.Fail(InvalidOperation.YouCannotUseThisObject);

        if (Metadata.OnUse?.GetAttribute(ItemAttribute.FloorChange) != "up")  return Result.Fail(InvalidOperation.YouCannotUseThisObject);

        var toLocation = new Location(onItem.Location.X, onItem.Location.Y, (byte)(onItem.Location.Z - 1));

        player.TeleportTo(toLocation);
        return Result.Success;
    }

    public new static bool IsApplicable(IItemType type)
    {
        return type.Group is ItemGroup.UsableFloorChanger;
    }
}