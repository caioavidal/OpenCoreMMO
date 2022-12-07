using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;

namespace NeoServer.Scripts.Lua.Patchers;

public class QuestPatcher: IPatcher
{
    public void Patch()
    {
        var allClasses = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
            .Where(x => x.IsAssignableTo(typeof(IUsable)) && x.IsClass && !x.IsAbstract)
            .ToHashSet();

        var harmony = new Harmony("com.opc.patch");

        foreach (var type in allClasses)
        {
            var originalMethod = type.GetMethod("Use",
                types: new[]{ typeof(IPlayer), typeof(byte) }, bindingAttr: BindingFlags.Instance | BindingFlags.Public);

            if (originalMethod is null) continue;
            
            if (originalMethod?.DeclaringType != type)
            {
                originalMethod = originalMethod?.DeclaringType?.GetMethod("Use",
                    types: new[]{ typeof(IPlayer), typeof(byte) }, bindingAttr: BindingFlags.Instance | BindingFlags.Public);
            }
            
            if (originalMethod is null) continue;
            
            var methodPrefix = typeof(QuestPatcher).GetMethod(nameof(Prefix), BindingFlags.Static | BindingFlags.NonPublic);

            if (methodPrefix is null) continue;

            harmony.Patch(originalMethod, new HarmonyMethod(methodPrefix));
        }
    }

    private static bool Prefix(IPlayer usedBy, byte openAtIndex, IThing __instance)
    {
        if (__instance is not IItem item) return false;
        var key = $"{item.Metadata.ActionId}-{item.Metadata.UniqueId}";
        
        var action = ItemActionMap.Get(key, "use");

        if (action is null) return true; //continue to original method
        
        action.Call(__instance, usedBy);

        return false;
    }
}