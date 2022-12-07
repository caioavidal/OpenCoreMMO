using System;
using System.Collections.Generic;
using NLua;


namespace NeoServer.Scripts.Lua;

public static class ItemActionMap
{
    private static readonly Dictionary<string, LuaFunction> Actions = new(StringComparer.InvariantCultureIgnoreCase);

    public static void Register(object key, string eventName, LuaFunction action)
    {
        Actions[$"{key}-{eventName}"] = action;
    }
    public static LuaFunction Get(string key, string eventName) => Actions.GetValueOrDefault($"{key}-{eventName}"); 
}

