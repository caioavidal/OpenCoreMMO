using System;
using System.Collections.Generic;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Texts;
using NeoServer.Game.Items.Items.UsableItems;
using NeoServer.Game.World.Map;
using NeoServer.Game.World.Services;

namespace NeoServer.Extensions.Items.Tools
{
    public class Shovel : TransformerUsableItem
    {
        public new static bool IsApplicable(IItemType type)
        {
            return UsableOnItem.IsApplicable(type) &&
                   (type.OnUse?.HasAttribute(ItemAttribute.TransformTo) ?? false) &&
                   (type.ClientId == 2554);
        }

        public Shovel(IItemType type, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(type, location)
        {
        }
        
        public override bool Use(ICreature usedBy, IItem item)
        {
            var result = OpenCaveHole(usedBy, item);
            if (!result) OperationFailService.Display(usedBy.CreatureId, TextConstants.NOT_POSSIBLE);

            return result;
        }

        private bool OpenCaveHole(ICreature usedBy, IItem item)
        {
            if (Map.Instance[item.Location] is not IDynamicTile tile) return false;

            if (!Metadata.OnUse.TryGetAttribute<ushort>(ItemAttribute.UseOn, out var useOnId)) return false;
            
            if (tile.Ground?.ServerId != useOnId) return false;
            if (usedBy is not IPlayer player) return false;

            tile.Ground.Transform(player);

            Map.Instance.TryMoveCreature(usedBy, tile.Location);

            return true;
        }
    }
}