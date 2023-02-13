using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Scripts.Lua.Patchers.Base;

namespace NeoServer.Scripts.Lua.Patchers;

public class UsableItemPatcher : Patcher<UsableItemPatcher>
{
    protected override HashSet<Type> Types => AppDomain.CurrentDomain.GetAssemblies().AsParallel().SelectMany(x => x.GetTypes())
        .Where(x => x.IsAssignableTo(typeof(IUsable)) && x.IsAssignableTo(typeof(IItem)) && x.IsClass && !x.IsAbstract)
        .ToHashSet();

    protected override string MethodName => "Use";
    protected override Type[] Params => new[] { typeof(IPlayer) };
    protected override string PrefixMethodName => nameof(Prefix);

    private static bool Prefix(IPlayer usedBy, IItem __instance)
    {
        var key = $"{__instance.ServerId}-{__instance.Location}";
        var action = ItemActionMap.Get(key, "use");

        if (action is null) return true; //continue to original method

        action.Call(__instance, usedBy);

        return false;
    }
}