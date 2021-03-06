﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeoServer.Server.Configurations;
using NeoServer.Server.Helpers.Extensions;
using Newtonsoft.Json;
using Serilog.Core;

namespace NeoServer.Loaders.Spawns
{
    public class SpawnLoader
    {
        private readonly Game.World.World _world;
        private readonly Logger logger;
        private readonly ServerConfiguration serverConfiguration;

        public SpawnLoader(Game.World.World world, ServerConfiguration serverConfiguration, Logger logger)
        {
            _world = world;
            this.serverConfiguration = serverConfiguration;
            this.logger = logger;
        }

        public void Load()
        {
            logger.Step("Loading spawns...", "{n} spawns loaded", () =>
            {
                var spawnData = GetSpawnData();

                var spawns = spawnData.AsParallel().Select(x => SpawnConverter.Convert(x)).ToList();

                _world.LoadSpawns(spawns);
                return new object[] {spawns.Count};
            });
        }

        private IEnumerable<SpawnData> GetSpawnData()
        {
            var basePath = $"{serverConfiguration.Data}/world/";

            var spawnPath = Path.Combine(basePath, $"{serverConfiguration.OTBM.Replace(".otbm", "-spawn")}.json");
            if (!File.Exists(spawnPath)) return new List<SpawnData>();
            var jsonString = File.ReadAllText(spawnPath);
            var spawn = JsonConvert.DeserializeObject<IEnumerable<SpawnData>>(jsonString);
            return spawn;
        }
    }
}