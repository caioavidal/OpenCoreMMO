using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Items.Bases;
using NeoServer.Game.Items.Items.Containers.Container;

namespace NeoServer.Scripts.Lua.EventRegister.Binds;

public static class ItemFunctionBind
{
    /// <summary>
    ///     Sets up the Lua script functions for item usage.
    /// </summary>
    public static void Setup()
    {
        Container.UseFunction = (item, player, index) => LuaScriptCaller.Call("use", item, player, index);
        BaseItem.UseFunction = (item, player) => LuaScriptCaller.Call("use", item, player);
        IUsableOnItem.UseFunction = (item, player, onItem) => LuaScriptCaller.Call("use", item, player, onItem);
    }
}