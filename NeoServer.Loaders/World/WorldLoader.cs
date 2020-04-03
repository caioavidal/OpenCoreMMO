using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Item;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Location;
using NeoServer.OTB.Enums;
using NeoServer.OTBM;
using NeoServer.OTBM.Structure;
using NeoServer.Server.Map;

namespace NeoServer.Loaders.World
{
    public class WorldLoader
    {
        private readonly OTBMLoader otbmLoader;
        private readonly Func<ushort, IItem> itemFactory;
        private Server.World.World world;

        public WorldLoader(OTBMLoader otbmLoader, Func<ushort, IItem> itemFactory, Server.World.World world)
        {
            this.otbmLoader = otbmLoader;
            this.itemFactory = itemFactory;
            this.world = world;

        }
        public void Load()
        {
            var otbm = otbmLoader.Load();

            var tiles = GetTiles(otbm);

            foreach (var tile in tiles)
            {
                world.AddTile(tile);
            }

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
        }

        private IEnumerable<ITile> GetTiles(OTBM.Structure.OTBM otbm)
        {
            foreach (var tileNode in otbm.TileAreas.SelectMany(t => t.Tiles))
            {
                var tile = new Tile(tileNode.Coordinate);

                var items = GetItemsOnTile(tileNode);

                foreach (var item in items)
                {
                    tile.AddContent(item);
                }

                tile.SetFlag((TileFlag)tileNode.Flag);

                yield return tile;
            }

        }

        private IEnumerable<IItem> GetItemsOnTile(TileNode tileNode)
        {
            foreach (var itemNode in tileNode.Items)
            {
                var item = itemFactory?.Invoke(itemNode.ItemId);

                item.ThrowIfNull($"Failed to create item on {tileNode.Coordinate}");

                foreach (var attr in itemNode.ItemNodeAttributes)
                {
                    item.Attributes.Add((ItemAttribute)attr.AttributeName, attr.Value);
                }

                item.LoadedFromMap = true;

                if (item.CanBeMoved && tileNode.NodeType == NodeType.HouseTile)
                {
                    Console.WriteLine($"Warning] Moveable item with ID: {itemNode.ItemId} in house at position {tileNode.Coordinate}.");
                }
                else
                {
                    item.StartDecaying();
                    yield return item;
                }
            }

        }
    }
}