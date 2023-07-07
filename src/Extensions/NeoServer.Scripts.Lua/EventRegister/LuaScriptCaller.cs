using System.Linq;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Scripts.Lua.EventRegister;

public static class LuaScriptCaller
{
    public static bool Call(string eventName, IItem item, object param1 = null,
        object param2 = null, object param3 = null,
        object param4 = null, object param5 = null,
        object param6 = null, object param7 = null,
        object param8 = null, object param9 = null,
        object param10 = null)
    {
        if (LuaEventManager.FindItemScript(item, eventName.ToLower()) is { } script)
        {
            return (bool)(script.Call(item, param1, param2, param3, param4,
                    param5, param6, param7, param8, param9, param10)?
                .FirstOrDefault() ?? true);
        }

        return false; // continue to the original method
    }
}