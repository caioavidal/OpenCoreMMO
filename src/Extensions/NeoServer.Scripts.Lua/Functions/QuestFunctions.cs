using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NeoServer.Data.Entities;
using NeoServer.Data.Repositories;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Server.Helpers;
using Serilog.Core;

namespace NeoServer.Scripts.Lua.Functions;

public static class QuestFunctions
{
    public static void AddQuestFunctions(this NLua.Lua lua)
    {
        lua.DoString("quest_helper = {}");
        lua["quest_helper.createRewards"] = GetRewards;

        lua["quest_helper.setQuestAsCompleted"] = SetQuestAsCompleted;
        lua["quest_helper.checkQuestCompleted"] = CheckIfQuestIsCompleted;
        lua["quest_helper.getQuestData"] = GetQuestData;
    }

    private static QuestData GetQuestData(IItem item)
    {
        var questStore = IoC.GetInstance<IQuestDataStore>();
        return questStore.Get((item.ActionId, item.UniqueId));
    }

    public static void RegisterQuests(NLua.Lua lua)
    {
        var questStore = IoC.GetInstance<IQuestDataStore>();

        if (!questStore.All.Any()) return;

        var quest = lua.GetTable("quest");

        if (quest is null) return;
        var register = lua.GetFunction("quest.register");
        if (register is null) return;

        var func = register;
        foreach (var questData in questStore.All)
        {
            if (!questData.AutoLoad) continue;
            func?.Call(questData.UniqueId);
        }
    }

    private static void SetQuestAsCompleted(IPlayer player, QuestData questData)
    {
        if (questData is null)
        {
            IoC.GetInstance<Logger>().Error("Quest data not found");
            return;
        }

        var repository = IoC.GetInstance<BaseRepository<PlayerQuestEntity>>();
        repository.Insert(new PlayerQuestEntity
        {
            Done = true,
            Name = questData.Name,
            ActionId = questData.ActionId,
            UniqueId = (int)questData.UniqueId,
            PlayerId = (int)player.Id,
            Group = questData.Group,
            GroupKey = questData.GroupKey
        }).Wait();
    }

    private static bool CheckIfQuestIsCompleted(IPlayer player, QuestData questData)
    {
        if (questData is null)
        {
            IoC.GetInstance<Logger>().Error("Quest data not found");
            return false;
        }

        var repository = IoC.GetInstance<BaseRepository<PlayerQuestEntity>>();

        var playerQuestModel = repository.NewDbContext.PlayerQuests
            .Where(x => (x.ActionId == questData.ActionId && x.UniqueId == questData.UniqueId) ||
                        (x.GroupKey != null && x.GroupKey == questData.GroupKey))
            .FirstOrDefaultAsync().Result;

        return playerQuestModel is not null && playerQuestModel.Done;
    }

    private static IItem[] GetRewards(QuestData questData)
    {
        if (questData is null) return null;
        if ((questData.Rewards is null || !questData.Rewards.Any()) && questData.ActionId == 0) return null;

        var rewards = questData.Rewards;

        var itemFactory = IoC.GetInstance<IItemFactory>();

        if (rewards is not null && rewards.Any())
        {
            var items = CreateRewards(itemFactory, rewards.ToList());
            return items;
        }

        var item = itemFactory.Create(questData.ActionId, Location.Zero, new Dictionary<ItemAttribute, IConvertible>
        {
            [ItemAttribute.Amount] = 1
        });
        if (item is null) return Array.Empty<IItem>();

        return new[]
        {
            item
        };
    }

    private static IItem[] CreateRewards(IItemFactory itemFactory, List<QuestData.Reward> rewards)
    {
        if (rewards is null) return Array.Empty<IItem>();

        var items = new List<IItem>();

        foreach (var reward in rewards)
        {
            var item = itemFactory.Create(reward.ItemId, Location.Zero, new Dictionary<ItemAttribute, IConvertible>
            {
                [ItemAttribute.Count] = reward.Amount == 0 ? 1 : reward.Amount
            });

            if (item is null) continue;

            items.Add(item);

            var childrenItems = CreateRewards(itemFactory, reward.Children.ToList());
            if (item is IContainer container)
                foreach (var createdItem in childrenItems)
                    container.AddItem(createdItem);
            else
                items.AddRange(childrenItems);
        }

        return items.ToArray();
    }
}