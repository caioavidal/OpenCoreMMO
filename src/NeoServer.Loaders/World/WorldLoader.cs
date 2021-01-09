using NeoServer.Game.Common;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.World.Map;
using NeoServer.Game.World.Map.Tiles;
using NeoServer.OTB.Enums;
using NeoServer.OTB.Parsers;
using NeoServer.OTBM;
using NeoServer.OTBM.Structure;
using NeoServer.Server.Standalone;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeoServer.Loaders.World
{
    public class WorldLoader
    {
        private Game.World.World world;
        private readonly Logger logger;
        private readonly IItemFactory itemFactory;
        private readonly ServerConfiguration serverConfiguration;

        public WorldLoader(IMap map, Game.World.World world, Logger logger, IItemFactory itemFactory, ServerConfiguration serverConfiguration)
        {
            this.world = world;
            this.logger = logger;
            this.itemFactory = itemFactory;
            this.serverConfiguration = serverConfiguration;
        }
        public void Load()
        {
            var fileStream = File.ReadAllBytes($"{serverConfiguration.Data}/world/{ serverConfiguration.OTBM}");

            var otbmNode = OTBBinaryTreeBuilder.Deserialize(fileStream);

            var otbm = new OTBMNodeParser().Parse(otbmNode);

            LoadTiles(otbm);

            foreach (var townNode in otbm.Towns)
            {
                world.AddTown(new Town()
                {
                    Id = townNode.Id,
                    Name = townNode.Name,
                    Coordinate = townNode.Coordinate
                });
            }
            foreach (var waypointNode in otbm.Waypoints)
            {
                world.AddWaypoint(new Waypoint
                {
                    Coordinate = waypointNode.Coordinate,
                    Name = waypointNode.Name
                });
            }

            logger.Information("{tiles} tiles, {towns} towns and {waypoints} waypoints loaded", world.LoadedTilesCount, world.LoadedTownsCount, world.LoadedWaypointsCount);

        }

        private void LoadTiles(OTBM.Structure.OTBM otbm)
        {
            otbm.TileAreas.SelectMany(t => t.Tiles)
                .ToList().ForEach(tileNode =>
                {

                    var items = GetItemsOnTile(tileNode).ToArray();

                    var tile = TileFactory.CreateTile(tileNode.Coordinate, (TileFlag)tileNode.Flag, items);

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
                    {
                        attributes.TryAdd((ItemAttribute)attr.AttributeName, attr.Value);
                    }
                }

                var item = itemFactory.Create(itemNode.ItemId, new Location(tileNode.Coordinate), attributes);

                item.ThrowIfNull($"Failed to create item on {tileNode.Coordinate}");

                // item.LoadedFromMap = true;

                if (item.CanBeMoved && tileNode.NodeType == NodeType.HouseTile)
                {
                    //yield return item;
                    //logger.Warning($"Moveable item with ID: {itemNode.ItemId} in house at position {tileNode.Coordinate}.");
                }
                else
                {
                    //item.StartDecaying();
                    //yield return item;
                }
                items[i++] = item;

            }

            return items;
        }
    }
}