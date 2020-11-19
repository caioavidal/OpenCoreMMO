using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeoServer.Loaders.Spawns
{
    public class SpawnLoader
    {
        private readonly Game.World.World _world;
        public SpawnLoader(Game.World.World world) => _world = world;
        public void Load()
        {
            var spawnData = GetSpawnData();

            var spawns = spawnData.Select(x => SpawnConverter.Convert(x)).ToList();

            _world.LoadSpawns(spawns);
        }

        private IEnumerable<SpawnData> GetSpawnData()
        {
            var basePath = "./data/world/";
            var jsonString = File.ReadAllText(Path.Combine(basePath, "spawn.json"));
            var spawn = JsonConvert.DeserializeObject<IEnumerable<SpawnData>>(jsonString);
            return spawn;
        }
    }
}
