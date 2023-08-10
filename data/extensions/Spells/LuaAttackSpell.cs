using System.IO;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Combat.Attacks;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Results;
using NeoServer.Game.Systems.Combat.Attacks.Spell;
using NeoServer.Server.Configurations;
using NeoServer.Server.Helpers;

namespace NeoServer.Extensions.Spells;

public class LuaAttackSpell : AttackSpell
{
    public override uint Duration => 2000;
    public override DamageType DamageType => DamageType.Physical;
    public override ConditionType ConditionType => ConditionType.None;
    public override EffectT Effect => EffectT.GlitterBlue;

    public override Result CanCast(ICombatActor actor)
    {
        return Result.Success;
    }

    public override ISpellCombatAttack CombatAttack { get; } = new SpellCombatAttack();

    public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
    {
        var dataPath = IoC.GetInstance<ServerConfiguration>().Data;

        error = InvalidOperation.None;
        var lua = IoC.GetInstance<NLua.Lua>();
        lua.DoFile(Path.Combine(dataPath, "scripts/spells/test.lua"));

        if (lua["onCastSpell"] is not NLua.LuaFunction luaFunction) return false;

        luaFunction.Call(this, actor, words);

        return true;
    }
}