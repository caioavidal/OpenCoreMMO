using NeoServer.Game.Contracts.Spells;
using NeoServer.Game.Creatures.Spells;
using NeoServer.Server.Compiler;
using NeoServer.Server.Standalone;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            var path = Path.Combine(serverConfiguration.Data, "spells", "spells.json");
            var jsonString = File.ReadAllText(path);
            var spells = JsonConvert.DeserializeObject<List<IDictionary<string, object>>>(jsonString);

            foreach (var spell in spells)
            {
                var type = ScriptList.Assemblies.FirstOrDefault(x => x.Key == spell["script"].ToString()).Value;
                var spellInstance = Activator.CreateInstance(type, true) as ISpell;

                spellInstance.Name = spell["name"].ToString();
                spellInstance.Cooldown = Convert.ToUInt32(spell["cooldown"]);
                spellInstance.Mana = Convert.ToUInt16(spell["mana"]);
                spellInstance.MinLevel = Convert.ToUInt16(spell["level"]);
                spellInstance.Vocations = spell.ContainsKey("vocations") ? (spell["vocations"] as JArray).Select(jv => (byte)jv).ToArray() : null;

                SpellList.Add(spell["words"].ToString(), spellInstance);
            }
        }
    }
}
