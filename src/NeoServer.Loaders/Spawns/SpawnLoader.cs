using NeoServer.Server.Standalone;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeoServer.Loaders.Spawns
{
    public class SpawnLoader
    {
        private readonly Game.World.World _world;
        private readonly ServerConfiguration serverConfiguration;
        public SpawnLoader(Game.World.World world, ServerConfiguration serverConfiguration)
        {
            _world = world;
            this.serverConfiguration = serverConfiguration;
        }
        public void Load()
        {
            var spawnData = GetSpawnData();

            var spawns = spawnData.Select(x => SpawnConverter.Convert(x)).ToList();

            _world.LoadSpawns(spawns);
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
