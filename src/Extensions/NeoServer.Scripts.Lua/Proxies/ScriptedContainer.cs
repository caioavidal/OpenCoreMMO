using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Items.Containers.Container;
using NeoServer.Scripts.Lua.EventRegister;

namespace NeoServer.Scripts.Lua.Proxies;

public class ScriptedContainer : Container
{
    public ScriptedContainer(IItemType type, Location location, IEnumerable<IItem> children = null) : base(type,
        location, children)
    {
    }

    public override void Use(IPlayer usedBy, byte openAtIndex)
    {
        var luaAction = LuaEventManager.FindScript(this, "use");

        if (luaAction is null)
        {
            base.Use(usedBy, openAtIndex);
            return;
        }

        luaAction.Call(this, usedBy, openAtIndex);
    }
}