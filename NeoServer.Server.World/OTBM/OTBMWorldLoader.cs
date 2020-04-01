using NeoServer.Game.Contracts.Item;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Server.Contracts;
using NeoServer.Server.Map;
using NeoServer.Server.Model.Items;
using NeoServer.Server.World.OTB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Server.World
{
    /// <summary>
    /// This class contains the methods necessary to load a .otbm file.
    /// </summary>
    public class OTBMWorldLoader
    {

        private readonly Func<ushort, IItem> itemFactory;
        private readonly World world;
        private readonly IDispatcher dispatcher;

        public OTBMWorldLoader(Func<ushort, IItem> itemFactory, World world, IDispatcher dispatcher)
        {
            this.itemFactory = itemFactory;
            this.world = world;
            this.dispatcher = dispatcher;
        }
        /// <summary>
        /// This class only support items encoded using this major version.
        /// </summary>
        public const uint SupportedItemEncodingMajorVersion = 3;

        /// <summary>
        /// This class only support items encoded using this minor version.
        /// </summary>
        public const uint SupportedItemEncodingMinorVersion = 8;

        /// <summary>
        /// Loads a .otbm file, parse it's contents and returns a <see cref="NeoServer.Server.World.World"/>.
        /// </summary>
        public World LoadWorld(ReadOnlyMemory<byte> serializedWorldData)
        {

            var rootNode = OTBDeserializer.DeserializeOTBData(
                serializedOTBData: serializedWorldData);

            ParseOTBTreeRootNode(rootNode);

            var worldDataNode = rootNode.Children[0];
            ParseWorldDataNode(worldDataNode, world);

            //Console.WriteLine($"Tiles Loaded: {world.c}");

            return world;
        }

        /// <summary>
        /// Logs the information embedded in the root node of the OTB tree.
        /// </summary>
        private void ParseOTBTreeRootNode(OTBNode rootNode)
        {
            if (rootNode == null)
                throw new ArgumentNullException(nameof(rootNode));
            if (rootNode.Children.Count != 1)
                throw new InvalidOperationException();

            var parsingStream = new OTBParsingStream(rootNode.Data);

            var headerVersion = parsingStream.ReadUInt32();
            //if (headerVersion == 0 || headerVersion > 2)
            //	throw new InvalidOperationException();

            var worldWidth = parsingStream.ReadUInt16();
            var worldHeight = parsingStream.ReadUInt16();

            var itemEncodingMajorVersion = parsingStream.ReadUInt32();
            //if (itemEncodingMajorVersion != SupportedItemEncodingMajorVersion)
            //	throw new InvalidOperationException();

            var itemEncodingMinorVersion = parsingStream.ReadUInt32();
            //if (itemEncodingMinorVersion < SupportedItemEncodingMinorVersion)
            //	throw new InvalidOperationException();

            //TODO
            //_logger.Info($"OTBM header version: {headerVersion}.");
            //_logger.Info($"World width: {worldWidth}.");
            //_logger.Info($"World height: {worldHeight}.");
            //_logger.Info($"Item encoding major version: {itemEncodingMajorVersion}.");
            //_logger.Info($"Item encoding minor version: {itemEncodingMinorVersion}.");
        }


        /// <summary>
        /// Updates the <paramref name="world"/> with the data contained
        /// in <paramref name="worldDataNode"/>.
        /// </summary>
        private void ParseWorldDataNode(OTBNode worldDataNode, World world)
        {

            if (worldDataNode == null)
                throw new ArgumentNullException(nameof(worldDataNode));
            if (world == null)
                throw new ArgumentNullException(nameof(world));
            if (worldDataNode.Type != OTBNodeType.WorldData)
                throw new InvalidOperationException();

            var count = worldDataNode.Children.Count;

            foreach (var child in worldDataNode.Children)
            {
                switch (child.Type)
                {
                    case OTBNodeType.TileArea:
                        ParseTileAreaNode(child, world);
                        break;

                    case OTBNodeType.TownCollection:
                        ParseTownCollectionNode(child, world);
                        break;

                    case OTBNodeType.WayPointCollection:
                        ParseWaypointCollectionNode(child, world);
                        break;

                    case OTBNodeType.ItemDefinition:
                        throw new NotImplementedException("TFS didn't implement this. So didn't we.");

                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        /// Updates the <paramref name="world"/> with the data contained
        /// in <paramref name="tileAreaNode"/>.
        /// </summary>
        private void ParseTileAreaNode(OTBNode tileAreaNode, World world)
        {
            if (tileAreaNode == null)
                throw new ArgumentNullException(nameof(tileAreaNode));

            if (tileAreaNode.Type != OTBNodeType.TileArea)
                throw new InvalidOperationException();

            var stream = new OTBParsingStream(tileAreaNode.Data);

            var areaStartX = stream.ReadUInt16();
            var areaStartY = stream.ReadUInt16();
            var areaZ = (sbyte)stream.ReadByte();

            var areaStartPosition = new Coordinate(
                x: areaStartX,
                y: areaStartY,
                z: areaZ);

            foreach (var tileNode in tileAreaNode.Children)
            {
                ParseTileNode(
                    tilesAreaStartPosition: areaStartPosition,
                    tileNode: tileNode,
                    world: world);
            }
        }

        /// <summary>
        /// Updates the <paramref name="world"/> with the data contained
        /// in <paramref name="tileNode"/>.
        /// </summary>
        private void ParseTileNode(
            in Coordinate tilesAreaStartPosition,
            OTBNode tileNode,
            World world
            )
        {

            if (tileNode == null)
                throw new ArgumentNullException(nameof(tileNode));
            if (world == null)
                throw new ArgumentNullException(nameof(world));
            if (tileNode.Type != OTBNodeType.HouseTile && tileNode.Type != OTBNodeType.NormalTile)
                throw new InvalidOperationException("Unknown tile node.");

            var stream = new OTBParsingStream(tileNode.Data);

            // Finding the tiles "absolute coordinates"
            var xOffset = stream.ReadByte();
            var yOffset = stream.ReadByte();
            var tilePosition = tilesAreaStartPosition.Translate(
                xOffset: xOffset,
                yOffset: yOffset);

            // Currently there's no support for houses
            // Checking whether the tile belongs to a house
            // House house = null;
            if (tileNode.Type == OTBNodeType.HouseTile)
            {
                var houseId = stream.ReadUInt32();
                //todo mapio 117
                //house = HouseManager.Instance.CreateHouseOrGetReference(houseId);
            }

            // We create the tile early and mutate it along the method
            var tile = new Tile((ushort)tilePosition.X, (ushort)tilePosition.Y, tilePosition.Z, itemFactory);

            // Parsing the tile attributes
            var tileFlags = TileFlags.None;
            var tilesItems = new List<IItem>();

            if (!stream.IsOver)
            {
                var tileNodeAttribute = (OTBMWorldNodeAttribute)stream.ReadByte();

                switch (tileNodeAttribute)
                {

                    case OTBMWorldNodeAttribute.TileFlags:
                        {
                            var newFlags = (OTBMTileFlags)stream.ReadUInt32();
                            tileFlags = UpdateTileFlags(tileFlags, newFlags);
                            break;
                        }

                    case OTBMWorldNodeAttribute.Item:
                        {
                            var item = ParseItemData(stream);
#warning Not sure if this is the proper method
                            tile.AddContent(item);
                            break;
                        }

                    default: break;
                        //throw new Exception($"Unknown tile attribute: ${tileNodeAttribute} at pos: ${tilePosition.ToString()}.");
                }
            }

            // var items = tileNode.Children.Select(node => ParseTilesItemNode(node));
            var items = tileNode
                .Children
                .Select(node => new OTBParsingStream(node.Data))
                .Select(nodeStream => ParseItemData(nodeStream));

            foreach (var i in items)
            {
#warning Not sure if this is the proper method
                tile.AddContent(i);
            }
            world.AddTile(tile);
        }


        private TileFlags UpdateTileFlags(TileFlags oldFlags, OTBMTileFlags newFlags)
        {
            if ((newFlags & OTBMTileFlags.ProtectionZone) != 0)
                oldFlags |= TileFlags.ProtectionZone;
            else if ((newFlags & OTBMTileFlags.NoPvpZone) != 0)
                oldFlags |= TileFlags.NoPvpZone;
            else if ((newFlags & OTBMTileFlags.PvpZone) != 0)
                oldFlags |= TileFlags.PvpZone;

            if ((newFlags & OTBMTileFlags.NoLogout) != 0)
                oldFlags |= TileFlags.NoLogout;

            return oldFlags;
        }


        /// <summary>
        /// Updates the <paramref name="world"/> with the data contained
        /// in <paramref name="tileNode"/>.
        /// </summary>
        private void ParseTownCollectionNode(OTBNode townCollectionNode, World world)
        {
            if (townCollectionNode == null)
                throw new ArgumentNullException(nameof(townCollectionNode));
            if (world == null)
                throw new ArgumentNullException(nameof(world));

            foreach (var townNode in townCollectionNode.Children)
            {
                if (townNode.Type != OTBNodeType.Town)
                    throw new InvalidOperationException();

                var stream = new OTBParsingStream(townNode.Data);

                var townId = stream.ReadUInt32();
                // Implement Town and TownManager

                var townName = stream.ReadString();
                // Set town name

                var townTempleX = stream.ReadUInt16();
                var townTempleY = stream.ReadUInt16();
                var townTempleZ = stream.ReadByte();
                // Set town's temple
            }
        }

        /// <summary>
        /// Updates the <paramref name="world"/> with the data contained
        /// in <paramref name="tileNode"/>.
        /// </summary>
        private void ParseWaypointCollectionNode(OTBNode waypointCollection, World world)
        {
            if (waypointCollection == null)
                throw new ArgumentNullException(nameof(waypointCollection));
            if (world == null)
                throw new ArgumentNullException(nameof(world));

            foreach (var waypointNode in waypointCollection.Children)
            {
                if (waypointNode.Type != OTBNodeType.WayPoint)
                    throw new InvalidOperationException();

                var stream = new OTBParsingStream(waypointNode.Data);

                var waypointName = stream.ReadString();

                var waypointX = stream.ReadUInt16();
                var waypointY = stream.ReadUInt16();
                var waypointZ = stream.ReadBool();

                // Implement waypoints
            }
        }

        ///////////////////////////////////// items 
        ///



        private IItem ParseItemData(OTBParsingStream stream)
        {
            var newItemId = GetItemIdAndConvertPvpFieldsToPermanentFields(stream);

            var item = itemFactory(newItemId);
            if (item == null)
            {
                //_logger.Warn($"{nameof(ItemFactory)} was unable to create a item with id: {newItemId}."); TODO
                return null;
            }

            if (!stream.IsOver && item.Count > 0)
            {
                //var itemAttribute = (OTBMWorldItemAttribute)stream.ReadByte();
                //itemAttribute = (OTBMWorldItemAttribute)stream.ReadByte();
                //itemAttribute = (OTBMWorldItemAttribute)stream.ReadByte();
                //itemAttribute = (OTBMWorldItemAttribute)stream.ReadByte();
                //while (!stream.IsOver && itemAttribute != OTBMWorldItemAttribute.None && itemAttribute != OTBMWorldItemAttribute.None2) {
                //	switch (itemAttribute) {
                //		case OTBMWorldItemAttribute.Count:
                //		case OTBMWorldItemAttribute.RuneCharges:
                //		var count = stream.ReadByte();
                //		item.SetAmount(count);
                //		break;

                //		case OTBMWorldItemAttribute.ActionId:
                //		var actionId = stream.ReadUInt16();
                //		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.ActionId)} not implemented.");
                //		break;

                //		case OTBMWorldItemAttribute.UniqueId:
                //		var uniqueId = stream.ReadUInt16();
                //		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.UniqueId)} not implemented.");
                //		break;

                //		case OTBMWorldItemAttribute.Text:
                //		var text = stream.ReadString();
                //		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.Text)} not implemented.");
                //		break;

                //		case OTBMWorldItemAttribute.WrittenDate:
                //		var writtenDate = stream.ReadUInt32();
                //		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.WrittenDate)} not implemented.");
                //		break;

                //		case OTBMWorldItemAttribute.WrittenBy:
                //		var writtenBy = stream.ReadString();
                //		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.WrittenBy)} not implemented.");
                //		break;

                //		case OTBMWorldItemAttribute.Description:
                //		var anotherDescription = stream.ReadString();
                //		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.Description)} not implemented.");
                //		break;

                //		case OTBMWorldItemAttribute.Charges:
                //		var charges = stream.ReadUInt16();
                //		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.Charges)} not implemented.");
                //		break;

                //		case OTBMWorldItemAttribute.Duration:
                //		var duration = stream.ReadUInt32();
                //		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.Duration)} not implemented.");
                //		break;

                //		case OTBMWorldItemAttribute.DecayingState:
                //		var decayingState = stream.ReadByte();
                //		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.DecayingState)} not implemented.");
                //		break;

                //		case OTBMWorldItemAttribute.Name:
                //		var name = stream.ReadString();
                //		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.Name)} not implemented.");
                //		break;

                //		case OTBMWorldItemAttribute.Article:
                //		var article = stream.ReadString();
                //		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.Article)} not implemented.");
                //		break;

                //		case OTBMWorldItemAttribute.PluralName:
                //		var pluralName = stream.ReadString();
                //		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.Article)} not implemented.");
                //		break;

                //		case OTBMWorldItemAttribute.Weight:
                //		var weight = stream.ReadUInt32();
                //		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.Weight)} not implemented.");
                //		break;

                //		case OTBMWorldItemAttribute.Attack:
                //		var attack = stream.ReadUInt32();
                //		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.Attack)} not implemented.");
                //		break;

                //		case OTBMWorldItemAttribute.Defense:
                //		var defense = stream.ReadUInt32();
                //		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.Defense)} not implemented.");
                //		break;

                //		case OTBMWorldItemAttribute.ExtraDefense:
                //		var extraDefense = stream.ReadUInt32();
                //		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.ExtraDefense)} not implemented.");
                //		break;

                //		case OTBMWorldItemAttribute.Armor:
                //		var armor = stream.ReadUInt32();
                //		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.Armor)} not implemented.");
                //		break;

                //		case OTBMWorldItemAttribute.HitChance:
                //		var hitChance = stream.ReadUInt32();
                //		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.HitChance)} not implemented.");
                //		break;

                //		case OTBMWorldItemAttribute.ShootRange:
                //		var shootRange = stream.ReadUInt32();
                //		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.ShootRange)} not implemented.");
                //		break;

                //		case OTBMWorldItemAttribute.DepotId:
                //		var depotId = stream.ReadUInt32();
                //		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.DepotId)} not implemented.");
                //		break;

                //		case OTBMWorldItemAttribute.HouseDorId:
                //		var hourDoorId = stream.ReadUInt32();
                //		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.HouseDorId)} not implemented.");
                //		break;

                //		case OTBMWorldItemAttribute.SleeperGUID:
                //		var sleeperGUID = stream.ReadUInt32();
                //		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.SleeperGUID)} not implemented.");
                //		break;

                //		case OTBMWorldItemAttribute.SleepStart:
                //		var sleepStart = stream.ReadUInt32();
                //		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.SleepStart)} not implemented.");
                //		break;

                //		case OTBMWorldItemAttribute.TeleportDestination:
                //		var teleportDestination = stream.ReadUInt32();
                //		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.TeleportDestination)} not implemented.");
                //		break;

                //		case OTBMWorldItemAttribute.ContainerItems:
                //		throw new InvalidOperationException("TFS threw an exception here, so we're throwing too.");

                //		case OTBMWorldItemAttribute.CustomAttributes:
                //		var size = stream.ReadUInt64();
                //		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.CustomAttributes)} not implemented.");
                //		for (UInt64 i = 0; i < size; i++) {
                //			var key = stream.ReadString();
                //			var pos = (OTBMCustomAttributeValueType)stream.ReadByte();
                //			object value;

                //			switch (pos) {
                //				case OTBMCustomAttributeValueType.String:
                //				value = stream.ReadString();
                //				break;

                //				case OTBMCustomAttributeValueType.Int64:
                //				value = stream.ReadUInt64();
                //				break;

                //				case OTBMCustomAttributeValueType.Double:
                //				value = stream.ReadDouble();
                //				break;

                //				case OTBMCustomAttributeValueType.Bool:
                //				value = stream.ReadBool();
                //				break;

                //				default:
                //				value = new object();
                //				break;
                //			}
                //		}
                //		break;

                //		case OTBMWorldItemAttribute.AnotherDescription:
                //		throw new InvalidOperationException("TFS didn't implement this.");

                //		case OTBMWorldItemAttribute.ExtensionFile:
                //		throw new InvalidOperationException("TFS didn't implement this.");

                //		case OTBMWorldItemAttribute.TileFlags:
                //		throw new InvalidOperationException("TFS didn't implement this.");

                //		case OTBMWorldItemAttribute.Item:
                //		throw new InvalidOperationException("TFS didn't implement this.");

                //		case OTBMWorldItemAttribute.ExtensionFileForSpawns:
                //		throw new InvalidOperationException("TFS didn't implement this.");

                //		case OTBMWorldItemAttribute.ExtensionFileForHouses:
                //		throw new InvalidOperationException("TFS didn't implement this.");

                //		default:
                //		throw new InvalidOperationException($"Unknown {nameof(OTBMWorldItemAttribute)}.");
                //	}
                //}
            }

            return item;
        }

        private static ushort GetItemIdAndConvertPvpFieldsToPermanentFields(OTBParsingStream stream)
        {
            var originalItemId = stream.ReadUInt16();
            var newItemId = originalItemId;

            switch (originalItemId)
            {
                case (UInt16)OTBMWorldItemId.FireFieldPvpLarge:
                    newItemId = (UInt16)OTBMWorldItemId.FireFieldPersistentLarge;
                    break;

                case (UInt16)OTBMWorldItemId.FireFieldPvpMedium:
                    newItemId = (UInt16)OTBMWorldItemId.FireFieldPersistentMedium;
                    break;

                case (UInt16)OTBMWorldItemId.FireFieldPvpSmall:
                    newItemId = (UInt16)OTBMWorldItemId.FireFieldPersistentSmall;
                    break;

                case (UInt16)OTBMWorldItemId.EnergyFieldPvp:
                    newItemId = (UInt16)OTBMWorldItemId.EnergyFieldPersistent;
                    break;

                case (UInt16)OTBMWorldItemId.PoisonFieldPvp:
                    newItemId = (UInt16)OTBMWorldItemId.PoisonFieldPersistent;
                    break;

                case (UInt16)OTBMWorldItemId.MagicWall:
                    newItemId = (UInt16)OTBMWorldItemId.MagicWallPersistent;
                    break;

                case (UInt16)OTBMWorldItemId.WildGrowth:
                    newItemId = (UInt16)OTBMWorldItemId.WildGrowthPersistent;
                    break;

                default:
                    break;
            }

            //if (newItemId != originalItemId)
            //_logger.Warn($"Converted {(OTBMWorldItemId)originalItemId} to {(OTBMWorldItemId)newItemId}."); TODO

            return newItemId;
        }
    }
}
