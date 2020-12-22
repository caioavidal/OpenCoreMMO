using NeoServer.Server.Standalone;
using Newtonsoft.Json;
using Serilog.Core;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeoServer.Loaders.Spawns
{
    public class SpawnLoader
    {
        private readonly Game.World.World _world;
        private readonly ServerConfiguration serverConfiguration;
        private readonly Logger logger;
        public SpawnLoader(Game.World.World world, ServerConfiguration serverConfiguration, Logger logger)
        {
            _world = world;
            this.serverConfiguration = serverConfiguration;
            this.logger = logger;
        }
        public void Load()
        {
            var spawnData = GetSpawnData();

            var spawns = spawnData.AsParallel().Select(x => SpawnConverter.Convert(x)).ToList();

            _world.LoadSpawns(spawns);
            logger.Information($"{spawns.Count} spawns loaded!");
        }

        private IEnumerable<SpawnData> GetSpawnData()
        {
            var basePath = "./data/world/";

            var spawnPath = Path.Combine(basePath, $"{serverConfiguration.OTBM.Replace(".otbm", "-spawn")}.json");
            if (!File.Exists(spawnPath))
            {
                return new List<SpawnData>();
            }
            var jsonString = File.ReadAllText(spawnPath);
            var spawn = JsonConvert.DeserializeObject<IEnumerable<SpawnData>>(jsonString);
            return spawn;
        }
    }
}