using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NLua;

namespace NeoServer.Scripts.Lua.Patchers;

public class AttackPatcher: IPatcher
{
    public void Patch()
    {
        var allClasses = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
            .Where(x => x.IsAssignableTo(typeof(IWeaponItem)) && x.IsClass && !x.IsAbstract)
            .ToHashSet();

        var harmony = new Harmony("com.opc.patch");

        foreach (var type in allClasses)
        {
            var originalMethod = GetOriginalMethod(type);

            if (originalMethod.DeclaringType != type)
            {
                originalMethod = GetOriginalMethod(originalMethod.DeclaringType);
            }
            
            var methodPrefix = typeof(AttackPatcher).GetMethod(nameof(Prefix), BindingFlags.Static | BindingFlags.NonPublic);

            if (originalMethod is null || methodPrefix is null) continue;

            harmony.Patch(originalMethod, new HarmonyMethod(methodPrefix));
        }
    }

    private static MethodInfo GetOriginalMethod(Type type)
    {
        return type.GetMethod("Attack",
            types: new[]{ typeof(ICombatActor), typeof(ICombatActor), typeof(CombatAttackResult).MakeByRefType() }, bindingAttr: BindingFlags.Instance | BindingFlags.Public);
    }

    private static bool Prefix(ICombatActor actor, ICombatActor enemy, out CombatAttackResult combatResult, ref bool __result, IUsableOnItem __instance)
    {
        var action = ItemActionMap.Get(__instance.Metadata.TypeId, "onAttack");
        combatResult = new();
        
        if (action is null)
        {
            return true; //continue to original method
        }
        
        __result = (bool)(action.Call(__instance, actor, enemy, combatResult)?.FirstOrDefault() ?? false);

        return false;
    }
}