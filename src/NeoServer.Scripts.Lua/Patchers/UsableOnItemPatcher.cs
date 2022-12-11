using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Scripts.Lua.Patchers.Base;

namespace NeoServer.Scripts.Lua.Patchers;

public class UsableOnItemPatcher : Patcher<UsableOnItemPatcher>
{
    protected override HashSet<Type> Types => AppDomain.CurrentDomain.GetAssemblies().AsParallel().SelectMany(x => x.GetTypes())
        .Where(x => x.IsAssignableTo(typeof(IUsableOnItem)) && x.IsClass && !x.IsAbstract)
        .ToHashSet();

    protected override string MethodName => "Use";
    protected override Type[] Params => new[] { typeof(ICreature), typeof(IItem) };
    protected override string PrefixMethodName => nameof(Prefix);

    private static bool Prefix(ICreature usedBy, IItem onItem, ref bool __result, IUsableOnItem __instance)
    {
        var action = ItemActionMap.Get(__instance.Metadata.TypeId.ToString(), "useOnItem");

        if (action is null) return true; //continue to original method

        __result = (bool)(action.Call(__instance, usedBy, onItem)?.FirstOrDefault() ?? false);

        return false;
    }
}