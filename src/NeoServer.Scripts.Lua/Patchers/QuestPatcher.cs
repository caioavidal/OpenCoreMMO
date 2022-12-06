using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Item;
using NLua;

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
                types: new[]{ typeof(IPlayer) }, bindingAttr: BindingFlags.Instance | BindingFlags.Public);

            if (originalMethod.DeclaringType != type)
            {
                originalMethod = originalMethod.DeclaringType.GetMethod("Use",
                    types: new[]{ typeof(ICreature) }, bindingAttr: BindingFlags.Instance | BindingFlags.Public);
            }
            
            var methodPrefix = typeof(QuestPatcher).GetMethod(nameof(Prefix), BindingFlags.Static | BindingFlags.NonPublic);

            if (originalMethod is null || methodPrefix is null) continue;

            harmony.Patch(originalMethod, new HarmonyMethod(methodPrefix));
        }
    }

    private static bool Prefix(ICreature player, IUsable __instance)
    {
        var key = $"{__instance.Metadata.ActionId}-{__instance.Metadata.UniqueId}";
        
        var action = ItemActionMap.Get(key, "use");

        if (action is null) return true; //continue to original method
        
        action.Call(__instance, player);

        return false;
    }
}