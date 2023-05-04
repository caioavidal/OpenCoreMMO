using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Spells;
using NeoServer.Server.Configurations;
using NeoServer.Server.Helpers.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace NeoServer.Loaders.Spells;

public class SpellLoader
{
    private readonly IVocationStore _vocationStore;
    private readonly ILogger logger;
    private readonly ServerConfiguration serverConfiguration;

    public SpellLoader(ServerConfiguration serverConfiguration,
        IVocationStore vocationStore,
        ILogger logger)
    {
        this.serverConfiguration = serverConfiguration;
        _vocationStore = vocationStore;
        this.logger = logger;
    }

    public void Load()
    {
        LoadSpells();
    }

    private void LoadSpells()
    {
        logger.Step("Loading spells...", "{n} spells loaded", () =>
        {
            var path = Path.Combine(serverConfiguration.Data, "spells", "spells.json");
            var jsonString = File.ReadAllText(path);
            var spells = JsonConvert.DeserializeObject<List<IDictionary<string, object>>>(jsonString)?.ToList() ??
                         new List<IDictionary<string, object>>(0);

            var types = ScriptSearch.All.Where(x => typeof(ISpell).IsAssignableFrom(x)).ToList();

            foreach (var spell in spells)
            {
                if (spell is null) continue;

                var type = types.FirstOrDefault(x => x.Name == spell["script"].ToString());
                if (type is null) continue;

                if (CreateSpell(type) is not ISpell spellInstance) continue;

                spellInstance.Name = spell["name"].ToString();
                spellInstance.Cooldown = Convert.ToUInt32(spell["cooldown"]);
                spellInstance.Mana = Convert.ToUInt16(spell["mana"]);
                spellInstance.MinLevel = Convert.ToUInt16(spell["level"]);
                spellInstance.Vocations = LoadVocations(spell);
                SpellList.Add(spell["words"].ToString(), spellInstance);
            }

            return new object[] { spells.Count };
        });
    }

    private byte[] LoadVocations(IDictionary<string, object> spell)
    {
        if (!spell.ContainsKey("vocations")) return null;

        return (spell["vocations"] as JArray)?.Select(vocationJToken =>
        {
            var vocationValue = (string)vocationJToken;

            if (vocationValue is null) return (byte)0;

            if (byte.TryParse(vocationValue, out var vocation)) return vocation;

            return _vocationStore.All.FirstOrDefault(x =>
                x.Name
                    .Replace(" ", string.Empty)
                    .Equals(vocationValue
                            .Replace(" ", string.Empty),
                        StringComparison.InvariantCultureIgnoreCase))?.VocationType ?? 0;
        }).ToArray();
    }

    private static object CreateSpell(Type type)
    {
        var constructorExpression = Expression.New(type);
        var lambdaExpression = Expression.Lambda<Func<object>>(constructorExpression);
        var createHeadersFunc = lambdaExpression.Compile();
        return createHeadersFunc();
    }
}