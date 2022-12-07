using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Server.Helpers;

namespace NeoServer.Scripts.Lua.Functions;

public static class QuestFunctions
{
    public static void AddQuestFunctions(this NLua.Lua lua)
    {
        lua.DoString("questhelper = {}");
        lua["questhelper.createRewards"] = GetRewards;
    }

    private static List<IItem> GetRewards(QuestData questData)
    {
        if (questData is null) return null;
        if ((questData.Rewards is null || !questData.Rewards.Any()) && questData.ActionId == 0) return null;
        
        var rewards = questData.Rewards;

        var itemFactory = IoC.GetInstance<IItemFactory>();
        //var lua = IoC.GetInstance<NLua.Lua>();
        if (rewards is not null && rewards.Any())
        {
            var items = CreateRewards(itemFactory, rewards.ToList());
            // var luaTable = new LuaTable(1, lua);
            // for (int i = 0; i < items.Count; i++)
            // {
            //     luaTable[i] = items[i];
            // }
            //
            // return luaTable;
            return items;
        }

        var item = itemFactory.Create(questData.ActionId, Location.Zero, new Dictionary<ItemAttribute, IConvertible>()
        {
            [ItemAttribute.Amount] = 1
        });
        if (item is null) return new List<IItem>(0);
        
        return new List<IItem>(1)
        {
            item
        };
    }

    private static List<IItem> CreateRewards(IItemFactory itemFactory, List<QuestData.Reward> rewards)
    {
        if (rewards is null) return new List<IItem>(0);
            
        var items = new List<IItem>();
        
        foreach (var reward in rewards)
        {
            var item = itemFactory.Create(reward.ItemId, Location.Zero, new Dictionary<ItemAttribute, IConvertible>
            {
                [ItemAttribute.Amount] = reward.Amount == 0 ? 1 : reward.Amount
            });

            if (item is null) continue;
            
            items.Add(item);

            var childrenItems = CreateRewards(itemFactory, reward.Children.ToList());
            if (item is IContainer container)
            {
                foreach (var createdItem in childrenItems)
                {
                    container.AddItem(createdItem);
                }
            }
            else
            {
                items.AddRange(childrenItems);    
            }
        }

        return items;
    }
}