using NeoServer.Game.Contracts.Spells;
using NeoServer.Game.Creatures.Spells;
using NeoServer.Server.Compiler;
using NeoServer.Server.Standalone;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeoServer.Loaders.Spells
{
    public class SpellLoader
    {
        private readonly ServerConfiguration serverConfiguration;
        public SpellLoader(ServerConfiguration serverConfiguration)
        {
            this.serverConfiguration = serverConfiguration;
        }
        public void Load()
        {
            LoadSpells();
        }
        private void LoadSpells()
        {
            var path = Path.Combine(serverConfiguration.Data,"spells", "spells.json");
            var jsonString = File.ReadAllText(path);
            var spells = JsonConvert.DeserializeObject<List<IDictionary<string, string>>>(jsonString);

            foreach (var spell in spells)
            {
                var type = ScriptList.Assemblies.FirstOrDefault(x => x.Key == spell["script"]).Value;
                var spellInstance = Activator.CreateInstance(type, true) as ISpell;

                spellInstance.Name = spell["name"];
                spellInstance.Cooldown = Convert.ToUInt32(spell["cooldown"]);
                spellInstance.Mana = Convert.ToUInt16(spell["mana"]);
                spellInstance.MinLevel = Convert.ToUInt16(spell["level"]);               

                SpellList.Add(spell["words"], spellInstance);
            }
        }
    }
}
