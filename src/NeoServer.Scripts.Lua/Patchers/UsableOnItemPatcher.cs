using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NLua;

namespace NeoServer.Scripts.Lua.Patchers;

public class UsableOnItemPatcher: IPatcher
{
    public void Patch()
    {
        var allClasses = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
            .Where(x => x.IsAssignableTo(typeof(IUsableOnItem)) && x.IsClass && !x.IsAbstract)
            .ToHashSet();

        var harmony = new Harmony("com.opc.patch");

        foreach (var type in allClasses)
        {
            var originalMethod = type.GetMethod("Use",
                types: new[]{ typeof(ICreature), typeof(IItem) }, bindingAttr: BindingFlags.Instance | BindingFlags.Public);

            if (originalMethod.DeclaringType != type)
            {
                originalMethod = originalMethod.DeclaringType.GetMethod("Use",
                    types: new[]{ typeof(ICreature), typeof(IItem) }, bindingAttr: BindingFlags.Instance | BindingFlags.Public);
            }
            
            var methodPrefix = typeof(UsableOnItemPatcher).GetMethod(nameof(Prefix), BindingFlags.Static | BindingFlags.NonPublic);

            if (originalMethod is null || methodPrefix is null) continue;

            harmony.Patch(originalMethod, new HarmonyMethod(methodPrefix));
        }
    }

    private static bool Prefix(ICreature usedBy, IItem onItem, ref bool __result, IUsableOnItem __instance)
    {
        var action = ItemActionMap.Get(__instance.Metadata.TypeId, "useOnItem");

        if (action is null) return true; //continue to original method
        
        __result = (bool) (action.Call(__instance, usedBy, onItem )?.FirstOrDefault() ?? false);

        return false;
    }
}