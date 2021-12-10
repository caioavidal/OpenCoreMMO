using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Texts;
using NeoServer.Game.Items.Bases;
using NeoServer.Game.Items.Factories;
using NeoServer.Game.World.Map;
using NeoServer.Game.World.Models.Tiles;

namespace NeoServer.Extensions.Items.Doors
{
    public class Door : BaseItem, IUsable
    {
        public Door(IItemType metadata, Location location, IDictionary<ItemAttribute, IConvertible> attributes) :
            base(metadata)
        {
            Location = location;
        }

        public virtual void Use(IPlayer player)
        {
            if (Map.Instance[Location] is not Tile tile) return;
            Console.WriteLine("oi");

            var mode = Metadata.Attributes.GetAttribute("mode");
            
            if (mode.Equals("closed", StringComparison.InvariantCultureIgnoreCase))
            {
                OpenDoor(tile);
                return;
            }
            if (mode.Equals("opened", StringComparison.InvariantCultureIgnoreCase))
            {
                CloseDoor(tile);
                return;
            }
            
            OperationFailService.Display(player.CreatureId, TextConstants.NOT_POSSIBLE);
        }

        private void OpenDoor(Tile tile)
        {
            var wallId = Metadata.Attributes.GetAttribute<ushort>("wall");
            var wall = tile.TopItems.ToList().FirstOrDefault(x => x.ServerId == wallId);

            if (!Metadata.Attributes.TryGetAttribute<ushort>(ItemAttribute.TransformTo, out var doorId)) return;

            if (wall is null) return;

            var door = ItemFactory.Instance.Create(doorId, Location, null);

            tile.RemoveItem(this, 1, out _);
            tile.RemoveItem(wall, 1, out _);

            tile.AddItem(door);
        }

        private void CloseDoor(Tile tile)
        {
            if (!Metadata.Attributes.TryGetAttribute<ushort>(ItemAttribute.TransformTo, out var doorId)) return;
            var door = ItemFactory.Instance.Create(doorId, Location, null);

            var wallId = door.Metadata.Attributes.GetAttribute<ushort>("wall");
            var wall = ItemFactory.Instance.Create(wallId, Location, null);

            if (wall is null) return;

            tile.RemoveItem(this, 1, out _);

            tile.AddItem(wall);
            tile.AddItem(door);
        }

        public static bool IsApplicable(IItemType type)
        {
            return type.Attributes.GetAttribute(ItemAttribute.Type) == "door";
        }
    }
}