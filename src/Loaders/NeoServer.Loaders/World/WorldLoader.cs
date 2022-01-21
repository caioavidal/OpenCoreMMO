using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.World.Factories;
using NeoServer.Game.World.Models;
using NeoServer.OTB.Enums;
using NeoServer.OTB.Parsers;
using NeoServer.OTBM.Loaders;
using NeoServer.OTBM.Structure.TileArea;
using NeoServer.Server.Configurations;
using NeoServer.Server.Helpers.Extensions;
using Serilog;

namespace NeoServer.Loaders.World
{
    public class WorldLoader
    {
        private readonly IItemFactory itemFactory;
        private readonly ILogger logger;
        private readonly ServerConfiguration serverConfiguration;
        private readonly Game.World.World world;

        public WorldLoader(IMap map, Game.World.World world, ILogger logger, IItemFactory itemFactory,
            ServerConfiguration serverConfiguration)
        {
            this.world = world;
            this.logger = logger;
            this.itemFactory = itemFactory;
            this.serverConfiguration = serverConfiguration;
        }

        public void Load()
        {
            logger.Step("Loading world...", "{tiles} tiles, {towns} towns and {waypoints} waypoints loaded", () =>
            {
                var fileStream = File.ReadAllBytes($"{serverConfiguration.Data}/world/{serverConfiguration.OTBM}");

                var otbmNode = OTBBinaryTreeBuilder.Deserialize(fileStream);

                var otbm = new OTBMNodeParser().Parse(otbmNode);

                LoadTiles(otbm);

                foreach (var townNode in otbm.Towns)
                    world.AddTown(new Town
                    {
                        Id = townNode.Id,
                        Name = townNode.Name,
                        Coordinate = townNode.Coordinate
                    });
                foreach (var waypointNode in otbm.Waypoints)
                    world.AddWaypoint(new Waypoint
                    {
                        Coordinate = waypointNode.Coordinate,
                        Name = waypointNode.Name
                    });

                return new object[] {world.LoadedTilesCount, world.LoadedTownsCount, world.LoadedWaypointsCount};
            });
        }

        private void LoadTiles(OTBM.Structure.OTBM otbm)
        {
            otbm.TileAreas.SelectMany(t => t.Tiles)
                .ToList().ForEach(tileNode =>
                {
                    var items = GetItemsOnTile(tileNode).ToArray();

                    var tile = TileFactory.CreateTile(tileNode.Coordinate, (TileFlag) tileNode.Flag, items);

                    world.AddTile(tile);
                    // return tile;
                });
        }

        private Span<IItem> GetItemsOnTile(TileNode tileNode)
        {
            Span<IItem> items = new IItem[tileNode.Items.Count];
            var i = 0;
            foreach (var itemNode in tileNode.Items)
            {
                IDictionary<ItemAttribute, IConvertible> attributes = null;
                if (itemNode.ItemNodeAttributes != null)
                {
                    attributes = new Dictionary<ItemAttribute, IConvertible>();
                    foreach (var attr in itemNode.ItemNodeAttributes)
                        attributes.TryAdd((ItemAttribute) attr.AttributeName, attr.Value);
                }

                var item = itemFactory.Create(itemNode.ItemId, new Location(tileNode.Coordinate), attributes);

                if (item.IsNull())
                {
                    logger.Error($"Failed to create item on {tileNode.Coordinate}", tileNode.Coordinate);
                    continue;
                }

                // item.LoadedFromMap = true;

                if (item.CanBeMoved && tileNode.NodeType == NodeType.HouseTile)
                {
                    //yield return item;
                    //logger.Warning($"Moveable item with ID: {itemNode.ItemType} in house at position {tileNode.Coordinate}.");
                }

                items[i++] = item;
            }

            return items;
        }
    }
}