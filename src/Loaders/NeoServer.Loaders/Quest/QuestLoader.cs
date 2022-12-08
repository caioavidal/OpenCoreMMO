using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Item;
using NeoServer.Server.Configurations;
using NeoServer.Server.Helpers.Extensions;
using Newtonsoft.Json;
using Serilog;

namespace NeoServer.Loaders.Quest;

public class QuestLoader
{
    private readonly IQuestStore _questStore;
    private readonly ILogger _logger;
    private readonly ServerConfiguration _serverConfiguration;

    public QuestLoader(ILogger logger,
        ServerConfiguration serverConfiguration, IQuestStore questStore)
    {
        _logger = logger;
        _serverConfiguration = serverConfiguration;
        _questStore = questStore;
    }

    public void Load()
    {
        _logger.Step("Loading quests...", "{n} quests loaded", () =>
        {
            var actions = GetQuests();
            actions.ForEach(x => _questStore.Add(x.Key, x));

            return new object[] { actions.Count };
        });
    }

    private List<QuestData> GetQuests()
    {
        var basePath = $"{_serverConfiguration.Data}";
        var jsonString = File.ReadAllText(Path.Combine(basePath, "quests.json"));
        var quests = JsonConvert.DeserializeObject<List<QuestModel>>(jsonString);

        return quests?.Select(x => new QuestData
        {
            Script = x.Script,
            ActionId = x.ActionId,
            UniqueId = x.UniqueId,
            Rewards = MapRewards(x.Rewards),
            Name = x.Name
        }).ToList();
    }

    private static QuestData.Reward[] MapRewards(List<QuestModel.Reward> rewards)
    {
        if (rewards is null) return Array.Empty<QuestData.Reward>();
        return rewards.Select(r => new QuestData.Reward()
        {
            Amount = r.Amount == 0 ? (byte)1 : r.Amount,
            Children = MapRewards(r.Children),
            ItemId = r.ItemId
        }).ToArray();
    }
}