using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Services;
using NeoServer.Game.Common.Texts;
using NeoServer.Game.Items.Items.UsableItems;
using NeoServer.Game.World.Map;

namespace NeoServer.Extensions.Items.Tools;

public class Shovel : TransformerUsableItem
{
    public Shovel(IItemType type, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(
        type, location)
    {
    }

    public new static bool IsApplicable(IItemType type)
    {
        return UsableOnItem.IsApplicable(type) &&
               (type.OnUse?.HasAttribute(ItemAttribute.TransformTo) ?? false) &&
               type.ClientId == 2554;
    }

    public override bool Use(ICreature usedBy, IItem onItem)
    {
        if (!CanUse(usedBy, onItem))
        {
            OperationFailService.Send(usedBy.CreatureId, TextConstants.NOT_POSSIBLE);
            return false;
        }

        var result = OpenCaveHole(usedBy, onItem);
        if (!result) OperationFailService.Send(usedBy.CreatureId, TextConstants.NOT_POSSIBLE);

        return result;
    }

    private bool OpenCaveHole(ICreature usedBy, IItem item)
    {
        if (Map.Instance[item.Location] is not IDynamicTile tile) return false;
        if (tile.Ground is null) return false;

        var useOnItems = Metadata.OnUse.GetAttributeArray<ushort>(ItemAttribute.UseOn);
        if (useOnItems == default) return false;

        if (!useOnItems.Contains(tile.Ground.ServerId)) return false;
        if (usedBy is not IPlayer player) return false;

        tile.Ground.Transform(player);

        tile.Ground.Decayable?.StartDecay();

        Map.Instance.TryMoveCreature(usedBy, tile.Location);

        return true;
    }
}