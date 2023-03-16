using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Scripts.Lua.Patchers.Base;

namespace NeoServer.Scripts.Lua.Patchers;

public class AttackPatcher : Patcher<AttackPatcher>
{
    protected override HashSet<Type> Types => AppDomain.CurrentDomain.GetAssemblies().AsParallel()
        .SelectMany(x => x.GetTypes())
        .Where(x => x.IsAssignableTo(typeof(IWeaponItem)) && x.IsClass && !x.IsAbstract)
        .ToHashSet();

    protected override string MethodName => "Attack";

    protected override Type[] Params => new[]
        { typeof(ICombatActor), typeof(ICombatActor), typeof(CombatAttackResult).MakeByRefType() };

    protected override string PrefixMethodName => nameof(Prefix);

    private static bool Prefix(ICombatActor actor, ICombatActor enemy, out CombatAttackResult combatResult,
        ref bool __result, IUsableOnItem __instance)
    {
        var action = ItemActionMap.Get(__instance.Metadata.TypeId.ToString(), "onAttack");
        combatResult = new CombatAttackResult();

        if (action is null) return true; //continue to original method

        __result = (bool)(action.Call(__instance, actor, enemy, combatResult)?.FirstOrDefault() ?? false);

        return false;
    }
}