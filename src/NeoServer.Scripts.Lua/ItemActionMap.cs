using System.Collections.Generic;
using NLua;


namespace NeoServer.Scripts.Lua;

public static class ItemActionMap
{
    private static readonly Dictionary<string, LuaFunction> Actions = new();

    public static void Register(ushort typeId, string eventName, LuaFunction action)
    {
        Actions[$"{typeId}-{eventName}"] = action;
    }
    public static LuaFunction Get(ushort typeId, string eventName) => Actions.GetValueOrDefault($"{typeId}-{eventName}"); 
}

