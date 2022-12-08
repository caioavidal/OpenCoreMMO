using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Scripts.Lua.Patchers.Base;

namespace NeoServer.Scripts.Lua.Patchers;

public class ItemMovePatcher: Patcher<ItemMovePatcher>
{
    protected override HashSet<Type> Types => AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
        .Where(x => x.IsAssignableTo(typeof(IPlayer)) && x.IsClass && !x.IsAbstract)
        .ToHashSet();

    protected override string MethodName => "MoveItem";
    protected override Type[] Params => new[] { typeof(IItem), typeof(IHasItem), typeof(IHasItem),  typeof(byte),
        typeof(byte),
        typeof(byte?)};
    protected override string PrefixMethodName => nameof(Prefix);

    private static bool Prefix(IItem item, IHasItem source, IHasItem destination, byte amount, 
        byte fromPosition, byte? toPosition, ref Result<OperationResult<IItem>> __result,
        IPlayer __instance)
    {
        var key = $"{item.ServerId}-{item.Location}";

        var action = ItemActionMap.Get(key, "canMove");

        if (action is null) return true; //continue to original method
        
        var result = (bool)action.Call(__instance, item, source, destination, amount, fromPosition, toPosition).FirstOrDefault();

        if (result) return true; //continue to original method
        
        __result = Result<OperationResult<IItem>>.Fail(InvalidOperation.None);

        return false;
    }
}