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
    /// <param name="typeId">The type ID of the item to register the event for.</param>
    /// <param name="eventName">The name of the event to register.</param>
    /// <param name="action">The Lua function to execute when the event is triggered.</param>
    public static void Register(ushort typeId, string eventName, LuaFunction action)
    {
        RegisterUseOnItemEvent(typeId, eventName, action);

        RegisterUseEvent(typeId, eventName, action);
    }

    private static void RegisterUseEvent(ushort typeId, string eventName, LuaFunction action)
    {
        if (eventName != "use") return;

        IUsable.UseFunctionMap[typeId] = (instance, usedBy) => { action.Call(instance, usedBy); };
    }

    private static void RegisterUseOnItemEvent(ushort typeId, string eventName, LuaFunction action)
    {
        if (eventName != "useOnItem") return;

        IUsableOnItem.UseFunctionMap[typeId] = (instance, usedBy, onItem) =>
            (bool)(action.Call(instance, usedBy, onItem)?.FirstOrDefault() ?? false);
    }
}