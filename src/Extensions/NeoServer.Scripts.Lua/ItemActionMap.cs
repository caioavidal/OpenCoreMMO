using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Items.Items.UsableItems;
using NLua;

namespace NeoServer.Scripts.Lua;

public static class ItemActionMap
{
    private static readonly Dictionary<string, LuaFunction> Actions = new(StringComparer.InvariantCultureIgnoreCase);

    public static void Register(object key, string eventName, LuaFunction action)
    {
        if (eventName == "useOnItem")
        {
            UsableOnItem.UseMap.TryAdd(ushort.Parse(key.ToString()), (instance, usedBy, onItem )=> 
                (bool)(action.Call(instance, usedBy, onItem)?.FirstOrDefault() ?? false));
        }
        
        Actions[$"{key}-{eventName}"] = action;
    }

    public static LuaFunction Get(string key, string eventName)
    {
        return Actions.GetValueOrDefault($"{key}-{eventName}");
    }

    public static void Clear()
    {
        Actions.Clear();
    }
}