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
	public static partial class OTBMWorldLoader {

		/// <summary>
		/// NLog's documentation suggests that we should store a reference to the logger,
		/// instead of asking the LogManager for a new instance everytime we need to log something.
		/// </summary>
		//private static readonly Logger _logger = LogManager.GetCurrentClassLogger(); //todo: implement log

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
		public static World LoadWorld(ReadOnlyMemory<byte> serializedWorldData) {
			var world = new World();
			
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
		private static void ParseOTBTreeRootNode(OTBNode rootNode) {
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
		private static void ParseWorldDataNode(OTBNode worldDataNode, World world) {
			if (worldDataNode == null)
				throw new ArgumentNullException(nameof(worldDataNode));
			if (world == null)
				throw new ArgumentNullException(nameof(world));
			if (worldDataNode.Type != OTBNodeType.WorldData)
				throw new InvalidOperationException();

			foreach (var child in worldDataNode.Children) {
				switch (child.Type) {
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
		private static void ParseTileAreaNode(OTBNode tileAreaNode, World world) {
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

			foreach (var tileNode in tileAreaNode.Children) {
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
		private static void ParseTileNode(
			in Coordinate tilesAreaStartPosition,
			OTBNode tileNode,
			World world
			) {
			if (tileNode == null)
				throw new ArgumentNullException(nameof(tileNode));
			if (world == null)
				throw new ArgumentNullException(nameof(world));
			if (tileNode.Type != OTBNodeType.HouseTile && tileNode.Type != OTBNodeType.NormalTile)
				throw new InvalidOperationException();

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
			if (tileNode.Type == OTBNodeType.HouseTile) {
				var houseId = stream.ReadUInt32();
				//house = HouseManager.Instance.CreateHouseOrGetReference(houseId);
			}

			// We create the tile early and mutate it along the method
			var tile = new Tile((ushort)tilePosition.X, (ushort)tilePosition.Y, tilePosition.Z);

			// Parsing the tile attributes
			var tileFlags = TileFlags.None;
			var tilesItems = new List<Item>();

			var tileNodeAttribute = (OTBMWorldNodeAttribute)stream.ReadByte();
			switch (tileNodeAttribute) {

				case OTBMWorldNodeAttribute.TileFlags:
				var newFlags = (OTBMTileFlags)stream.ReadUInt32();
				tileFlags = UpdateTileFlags(tileFlags, newFlags);
				break;

				case OTBMWorldNodeAttribute.Item:
				var item = ParseItemData(stream);
#warning Not sure if this is the proper method
				tile.AddContent(item);
				break;

				default:
				throw new Exception("TFS just threw a exception here, so shall we... Reason: unknown tile attribute.");
			}

			// var items = tileNode.Children.Select(node => ParseTilesItemNode(node));
			var items = tileNode
				.Children
				.Select(node => new OTBParsingStream(node.Data))
				.Select(nodeStream => ParseItemData(nodeStream));

			foreach (var i in items) {
#warning Not sure if this is the proper method
				tile.AddContent(i);
			}
			world.AddTile(tile);
		}


		private static TileFlags UpdateTileFlags(TileFlags oldFlags, OTBMTileFlags newFlags) {
			if ((newFlags & OTBMTileFlags.NoLogout) != 0)
				oldFlags |= TileFlags.NoLogout;

			// I think we should throw if a tile contains contradictory flags, instead of just
			// ignoring them like tfs does...
			if ((newFlags & OTBMTileFlags.ProtectionZone) != 0)
				oldFlags |= TileFlags.ProtectionZone;
			else if ((newFlags & OTBMTileFlags.NoPvpZone) != 0)
				oldFlags |= TileFlags.NoPvpZone;
			else if ((newFlags & OTBMTileFlags.PvpZone) != 0)
				oldFlags |= TileFlags.PvpZone;

			return oldFlags;
		}


		/// <summary>
		/// Updates the <paramref name="world"/> with the data contained
		/// in <paramref name="tileNode"/>.
		/// </summary>
		private static void ParseTownCollectionNode(OTBNode townCollectionNode, World world) {
			if (townCollectionNode == null)
				throw new ArgumentNullException(nameof(townCollectionNode));
			if (world == null)
				throw new ArgumentNullException(nameof(world));

			foreach (var townNode in townCollectionNode.Children) {
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
		private static void ParseWaypointCollectionNode(OTBNode waypointCollection, World world) {
			if (waypointCollection == null)
				throw new ArgumentNullException(nameof(waypointCollection));
			if (world == null)
				throw new ArgumentNullException(nameof(world));

			foreach (var waypointNode in waypointCollection.Children) {
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
	}
}
