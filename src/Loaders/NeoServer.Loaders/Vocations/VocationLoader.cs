using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Creatures.Vocation;
using NeoServer.Server.Configurations;
using NeoServer.Server.Helpers.Extensions;
using NeoServer.Server.Helpers.JsonConverters;
using Newtonsoft.Json;
using Serilog;

namespace NeoServer.Loaders.Vocations;

public class VocationLoader
{
    public static VocationLoader Instance;

    private readonly ILogger _logger;
    private readonly ServerConfiguration _serverConfiguration;
    private readonly IVocationStore _vocationStore;

    public VocationLoader(ILogger logger,
        ServerConfiguration serverConfiguration, IVocationStore vocationStore)
    {
        _logger = logger;
        _serverConfiguration = serverConfiguration;
        _vocationStore = vocationStore;
        Instance = this;
    }

    public void Load()
    {
        _logger.Step("Loading vocations...", "{n} vocations loaded", () =>
        {
            _vocationStore.Clear();
            var vocations = GetVocations();
            foreach (var vocation in vocations) _vocationStore.Add(vocation.VocationType, vocation);

            return new object[] { vocations.Count };
        });
    }

    public void Reload()
    {
        _logger.Step("Reloading vocations...", "{n} vocations reloaded", () =>
        {
            var vocations = GetVocations();
            AddOrUpdateVocation(vocations);
            return new object[] { vocations.Count };
        });
    }

    private void AddOrUpdateVocation(List<Vocation> vocations)
    {
        foreach (var vocation in vocations)
        {
            if (_vocationStore.TryGetValue(vocation.VocationType, out var existingVocation))
            {
                UpdateVocation(existingVocation, vocation);

                continue;
            }

            _vocationStore.Add(vocation.VocationType, vocation);
        }
    }

    private static void UpdateVocation(IVocation existingVocation, IVocation vocation)
    {
        existingVocation.Clientid = vocation.Clientid;
        existingVocation.Description = vocation.Description;


        UpdateFormula(existingVocation, vocation);

        existingVocation.Id = vocation.Id;
        existingVocation.Name = vocation.Name;

        UpdateSkills(existingVocation, vocation);

        existingVocation.AttackSpeed = vocation.AttackSpeed;
        existingVocation.BaseSpeed = vocation.BaseSpeed;
        existingVocation.FromVoc = vocation.FromVoc;
        existingVocation.GainCap = vocation.GainCap;
        existingVocation.GainHp = vocation.GainHp;
        existingVocation.GainMana = vocation.GainMana;
        existingVocation.GainHpAmount = vocation.GainHpAmount;
        existingVocation.GainHpTicks = vocation.GainHpTicks;
        existingVocation.GainManaAmount = vocation.GainManaAmount;
        existingVocation.GainManaTicks = vocation.GainManaTicks;
        existingVocation.GainSoulTicks = vocation.GainSoulTicks;
        existingVocation.SoulMax = vocation.SoulMax;
    }

    private static void UpdateFormula(IVocation existingVocation, IVocation vocation)
    {
        if (vocation.Formula is null) return;

        existingVocation.Formula ??= new VocationFormula();

        existingVocation.Formula.Armor = (float)vocation.Formula?.Armor;
        existingVocation.Formula.Defense = (float)vocation.Formula?.Defense;
        existingVocation.Formula.DistDamage = (float)vocation.Formula?.DistDamage;
        existingVocation.Formula.MeleeDamage = (float)vocation.Formula?.MeleeDamage;
    }

    private static void UpdateSkills(IVocation existingVocation, IVocation vocation)
    {
        if (vocation.Skills is null) return;

        existingVocation.Skills ??= new Dictionary<SkillType, float>();
        foreach (var (key, value) in vocation.Skills) existingVocation.Skills.AddOrUpdate(key, value);
    }

    private List<Vocation> GetVocations()
    {
        var basePath = $"{_serverConfiguration.Data}";
        var jsonString = File.ReadAllText(Path.Combine(basePath, "vocations.json"));
        var vocations = JsonConvert.DeserializeObject<List<Vocation>>(jsonString, new JsonSerializerSettings
        {
            Converters =
            {
                new AbstractConverter<VocationFormula, IVocationFormula>(),
                new SkillConverter()
            }
        });
        return vocations;
    }

    private class SkillConverter : JsonConverter<Dictionary<SkillType, float>>
    {
        public override Dictionary<SkillType, float> ReadJson(JsonReader reader, Type objectType,
            [AllowNull] Dictionary<SkillType, float> existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            return serializer.Deserialize<List<Dictionary<string, string>>>(reader)?.ToDictionary(
                x => ParseSkillName(x["name"]),
                x => float.Parse(x["multiplier"], CultureInfo.InvariantCulture.NumberFormat));
        }

        private static SkillType ParseSkillName(string skillName)
        {
            return skillName switch
            {
                "fist" => SkillType.Fist,
                "axe" => SkillType.Axe,
                "sword" => SkillType.Sword,
                "club" => SkillType.Club,
                "shielding" => SkillType.Shielding,
                "fishing" => SkillType.Fishing,
                "distance" => SkillType.Distance,
                "magic" => SkillType.Magic,
                "level" => SkillType.Level,
                "speed" => SkillType.Speed,
                _ => throw new ArgumentOutOfRangeException(nameof(skillName), skillName, null)
            };
        }

        public override void WriteJson(JsonWriter writer, [AllowNull] Dictionary<SkillType, float> value,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}