using System.Linq;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NLua;

namespace NeoServer.Scripts.Lua.EventRegisters;

/// <summary>
///     A static class for registering Lua scripts to item methods.
/// </summary>
public static class ItemRegister
{
    /// <summary>
    ///     Register a Lua function to a given event of an item.
    /// </summary>
    /// <param name="key">The event's key of the item to register the event for.</param>
    /// <param name="eventName">The name of the event to register.</param>
    /// <param name="action">The Lua function to execute when the event is triggered.</param>
    public static void Register(string key, string eventName, LuaFunction action)
    {
        RegisterUseOnItemEvent(key, eventName, action);

        RegisterUseEvent(key, eventName, action);
    }

    /// <summary>
    ///     Register a Lua function to a given event of an item.
    /// </summary>
    /// <param name="id">The key of the item event to register the event for.</param>
    /// <param name="eventName">The name of the event to register.</param>
    /// <param name="action">The Lua function to execute when the event is triggered.</param>
    private static void RegisterUseEvent(string id, string eventName, LuaFunction action)
    {
        if (eventName != "use") return;

        IUsable.UseFunctionMap[id] = (instance, usedBy) => { action.Call(instance, usedBy); };
    }

    private static void RegisterUseOnItemEvent(string key, string eventName, LuaFunction action)
    {
        if (eventName != "useOnItem") return;

        IUsableOnItem.UseFunctionMap[key] = (instance, usedBy, onItem) =>
            (bool)(action.Call(instance, usedBy, onItem)?.FirstOrDefault() ?? false);
    }
}