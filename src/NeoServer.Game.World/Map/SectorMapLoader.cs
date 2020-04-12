//// <copyright file="SectorMapLoader.cs" company="2Dudes">
//// Copyright (c) 2018 2Dudes. All rights reserved.
//// Licensed under the MIT license.
//// See LICENSE file in the project root for full license information.
//// </copyright>

//using System;
//using System.IO;
//using NeoServer.Server.Model.World.Structs;
//using NeoServer.Game.World.Map;
//using StackExchange.Redis;

//namespace NeoServer.Game.World.Map
//{
//    public class SectorMapLoader : IMapLoader
//    {
//        // TODO: to configuration
//        private static readonly Lazy<ConnectionMultiplexer> _cacheConnectionInstance = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect("127.0.0.1:6379"));

//        public static ConnectionMultiplexer CacheConnection => _cacheConnectionInstance.Value;

//        public const int SectorXMin = 996;
//        public const int SectorXMax = 1043;

//        public const int SectorYMin = 984;
//        public const int SectorYMax = 1031;

//        public const int SectorZMin = 0;
//        public const int SectorZMax = 15;

//        private readonly DirectoryInfo _mapDirInfo;
//        private readonly bool[,,] _sectorsLoaded;

//        private long _totalTileCount;
//        private long _totalLoadedCount;

//        public byte PercentageComplete => (byte)Math.Floor(Math.Min(100, Math.Max((decimal)0, _totalLoadedCount * 100 / (_totalTileCount + 1))));

//        public SectorMapLoader(string mapFilesPath)
//        {
//            if (string.IsNullOrWhiteSpace(mapFilesPath))
//            {
//                throw new ArgumentNullException(nameof(mapFilesPath));
//            }

//            _mapDirInfo = new DirectoryInfo(mapFilesPath);

//            _totalTileCount = 1;
//            _totalLoadedCount = default;
//            _sectorsLoaded = new bool[SectorXMax - SectorXMin, SectorYMax - SectorYMin, SectorZMax - SectorZMin];
//        }


//        public bool HasLoaded(int x, int y, byte z)
//        {
//            if (x > SectorXMax)
//            {
//                return _sectorsLoaded[(x / 32) - SectorXMin, (y / 32) - SectorYMin, z - SectorZMin];
//            }

//            return _sectorsLoaded[x - SectorXMin, y - SectorYMin, z - SectorZMin];
//        }

//		public ITile GetTile(Location location) {
//			throw new NotImplementedException();
//		}
//	}
//}
