using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Scripts.Lua.Patchers.Base;
using NeoServer.Server.Helpers;

namespace NeoServer.Scripts.Lua.Patchers;

public class QuestPatcher: Patcher<QuestPatcher>
{
    protected override HashSet<Type> Types => AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
        .Where(x => x.IsAssignableTo(typeof(IUsable)) && x.IsClass && !x.IsAbstract)
        .ToHashSet();

    protected override string MethodName => "Use";
    protected override Type[] Params => new[] { typeof(IPlayer), typeof(byte) };
    protected override string PrefixMethodName => nameof(Prefix);

    private static bool Prefix(IPlayer usedBy, byte openAtIndex, IThing __instance)
    {
        if (__instance is not IItem item) return false;
        var key = $"{item.Metadata.ActionId}-{item.Metadata.UniqueId}";

        var action = ItemActionMap.Get(key, "use");

        if (action is null) return true; //continue to original method
        
        IoC.GetInstance<IQuestStore>().TryGetValue((item.Metadata.ActionId,item.Metadata.UniqueId), out var questData);

        action.Call(__instance, usedBy, questData);

        return false;
    }
}