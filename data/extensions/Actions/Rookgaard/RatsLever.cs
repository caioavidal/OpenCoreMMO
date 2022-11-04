using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Features.Metadata;
using NeoServer.Extensions.Items;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Factories;
using NeoServer.Game.World.Map;
using NeoServer.Game.World.Models.Tiles;

namespace NeoServer.Extensions.Actions.Rookgaard;

public class RatsLever : Lever
{
    private bool isOpened => Metadata.TypeId == 1946;

    public RatsLever(IItemType metadata, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(
        metadata, location, attributes)
    {
    }

    public override void Use(IPlayer player)
    {
        base.Use(player);
        SwitchOtherLever();

        if (isOpened)
        {
            RemoveBridge();
            return;
        }

        AddBridge();

    }

    private void SwitchOtherLever()
    {
        var firstLeverLocation = new Location(32098, 32204, 8);
        var secondLeverLocation = new Location(32104, 32204, 8);
        var locationOtherSwitch = Location == firstLeverLocation ? secondLeverLocation : firstLeverLocation;
        
        var lever = (Map.Instance[locationOtherSwitch] as DynamicTile)?.AllItems?.FirstOrDefault(x =>
            x.Metadata.TypeId is 1946 or 1945) as Lever;
        
        lever?.SwitchLever();
    }

    private static void AddBridge()
    {
        var tile1 = Map.Instance[32099, 32205, 8] as DynamicTile;
        var newGround1 = ItemFactory.Instance.Create(5770, tile1.Location, null) as IGround;

        var tile2 = Map.Instance[32100, 32205, 8] as DynamicTile;
        var newGround2 = ItemFactory.Instance.Create(5770, tile2.Location, null) as IGround;

        var tile3 = Map.Instance[32101, 32205, 8] as DynamicTile;
        var newGround3 = ItemFactory.Instance.Create(5770, tile3.Location, null) as IGround;

        tile1.RemoveStaticItems();
        tile1?.ReplaceGround(newGround1);

        tile2?.ReplaceGround(newGround2);

        tile3.RemoveStaticItems();
        tile3?.ReplaceGround(newGround3);
    }

    private static void RemoveBridge()
    {
        var tile1 = Map.Instance[32099, 32205, 8] as DynamicTile;
        var newGround1 = ItemFactory.Instance.Create(351, tile1.Location, null) as IGround;
        var newGroundBorder1 = ItemFactory.Instance.Create(4645, tile1.Location, null);

        var tile2 = Map.Instance[32100, 32205, 8] as DynamicTile;
        var newGround2 = ItemFactory.Instance.Create(4615, tile2.Location, null) as IGround;

        var tile3 = Map.Instance[32101, 32205, 8] as DynamicTile;
        var newGround3 = ItemFactory.Instance.Create(352, tile3.Location, null) as IGround;
        var newGroundBorder3 = ItemFactory.Instance.Create(4647, tile3.Location, null);
        
        TeleportAllCreatures(tile1);
        TeleportAllCreatures(tile2);
        TeleportAllCreatures(tile3);

        tile1.RemoveAllItems();
        tile1.ReplaceGround(newGround1);
        tile1.AddItem(newGroundBorder1);
        
        tile2.RemoveAllItems();
        tile2.ReplaceGround(newGround2);

        tile3.RemoveAllItems();
        tile3.ReplaceGround(newGround3);
        tile3.AddItem(newGroundBorder3);
    }

    private static void TeleportAllCreatures(IDynamicTile dynamicTile)
    {
        if (dynamicTile.Creatures is null) return;
        
        foreach (var creature in dynamicTile.Creatures.ToArray())
        {
            creature.TeleportTo(new Location(32102, 32205, 8));
        }
    }
}