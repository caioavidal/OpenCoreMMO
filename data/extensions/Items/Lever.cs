using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Bases;
using NeoServer.Game.Items.Factories;
using NeoServer.Game.World.Models.Tiles;

namespace NeoServer.Extensions.Items;

public class Lever: BaseItem, IUsable
{
    public Lever(IItemType metadata, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(metadata, location)
    {
    }

    public void Use(IPlayer player)
    {
       Console.WriteLine("used Lever");
    }

    private void SwitchLever(DynamicTile dynamicTile)
    {
        // if (!Metadata.Attributes.TryGetAttribute<ushort>(ItemAttribute.TransformTo, out var leverId)) return;
        //
        // var door = ItemFactory.Instance.Create(leverId, Location, null);
        //
        // dynamicTile.RemoveItem(this, 1, out _);
        //
        //
        //    dynamicTile.RemoveItem(wall, 1, out _);
        //
        //
        // dynamicTile.AddItem(door);
    }
}