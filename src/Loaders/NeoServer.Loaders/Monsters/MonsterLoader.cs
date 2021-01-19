using NeoServer.Game.Common;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Standalone;
using Newtonsoft.Json;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeoServer.Loaders.Monsters
{
    public class MonsterLoader
    {
        private readonly IMonsterDataManager _monsterManager;
        private readonly GameConfiguration gameConfiguration;
        private readonly ServerConfiguration serverConfiguration;
        private readonly Logger logger;
        public MonsterLoader(IMonsterDataManager monsterManager, GameConfiguration gameConfiguration, Logger logger, ServerConfiguration serverConfiguration)
        {
            _monsterManager = monsterManager;
            this.gameConfiguration = gameConfiguration;
            this.logger = logger;
            this.serverConfiguration = serverConfiguration;
        }
        public void Load()
        {
            var monsters = GetMonsterDataList().ToList();
            _monsterManager.Load(monsters);

            logger.Information("{n} monsters loaded!", monsters.Count());
        }

        private IEnumerable<(string,IMonsterType)> GetMonsterDataList()
        {
            var basePath = $"{serverConfiguration.Data}/monsters";
            var jsonString = File.ReadAllText(Path.Combine(basePath, "monsters.json"));
            var monstersPath = JsonConvert.DeserializeObject<List<IDictionary<string, string>>>(jsonString);

            return monstersPath.AsParallel().Select(x => (x["name"], ConvertMonster(basePath, x)));
        }
        private IMonsterType ConvertMonster(string basePath, IDictionary<string, string> monsterFile)
        {
            var json = File.ReadAllText(Path.Combine(basePath, monsterFile["file"]));

            var monster = JsonConvert.DeserializeObject<MonsterData>(json, new JsonSerializerSettings { Error = (se, ev) => { ev.ErrorContext.Handled = true; Console.WriteLine(ev.ErrorContext.Error); } });

            return MonsterConverter.Convert(monster, gameConfiguration, _monsterManager);
        }
    }
}