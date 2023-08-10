using System;
using System.IO;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Combat.Attacks;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Systems.Combat.Attacks.Spell;
using NeoServer.Server.Configurations;
using NeoServer.Server.Helpers;

namespace NeoServer.Extensions.Spells.Attack;

public class LuaAttackSpell : AttackSpell
{
    public override byte Range => 1;
    public override bool NeedsTarget => false;
    public override DamageType DamageType => DamageType.Holy;
    public override string AreaName => "AREA_CIRCLE3X3";

    public override MinMax GetFormula(ICombatActor actor)
    {
        var dataPath = IoC.GetInstance<ServerConfiguration>().Data;

        var lua = IoC.GetInstance<NLua.Lua>();
        lua.DoFile(Path.Combine(dataPath, "scripts/spells/test.lua"));

        if (lua["getFormula"] is not NLua.LuaFunction luaFunction) return new MinMax(0, 0);

        var result = luaFunction.Call(this, actor);
        if (result is null || result.Length == 0) return new MinMax(0, 0);

        var min = Convert.ToInt32(result[0]);
        var max = result.Length > 1 ? Convert.ToInt32(result[1]) : min;

        return new MinMax(min, max);
    }

    public override ISpellCombatAttack CombatAttack { get; } = new SpellCombatAttack();
}