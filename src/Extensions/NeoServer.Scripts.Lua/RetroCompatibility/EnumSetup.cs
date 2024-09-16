using NeoServer.Application.Features.Combat.Definition;
using NeoServer.Game.Common.Item;

namespace NeoServer.Scripts.Lua.RetroCompatibility;

public class EnumSetup
{
    public static void Register(NLua.Lua lua)
    {
        RegisterDamageEnum(lua);
        RegisterCombatAttributeEnum(lua);
        RegisterCombatFormulaEnum(lua);
    }

    private static void RegisterCombatFormulaEnum(NLua.Lua lua)
    {
        EnumRegister.Register(lua, "COMBAT_FORMULA_UNDEFINED", CombatFormula.None);
        EnumRegister.Register(lua, "COMBAT_FORMULA_LEVELMAGIC", CombatFormula.MagicLevel);
        EnumRegister.Register(lua, "COMBAT_FORMULA_SKILL", CombatFormula.Skill);
        EnumRegister.Register(lua, "COMBAT_FORMULA_DAMAGE", CombatFormula.Damage);
    }

    private static void RegisterCombatAttributeEnum(NLua.Lua lua)
    {
        EnumRegister.Register(lua, "COMBAT_PARAM_TYPE", CombatAttribute.Damage);
        EnumRegister.Register(lua, "COMBAT_PARAM_EFFECT", CombatAttribute.Effect);
        EnumRegister.Register(lua, "COMBAT_PARAM_DISTANCEEFFECT", CombatAttribute.DistanceEffect);
        EnumRegister.Register(lua, "COMBAT_PARAM_BLOCKSHIELD", CombatAttribute.BlockShield);
        EnumRegister.Register(lua, "COMBAT_PARAM_BLOCKARMOR", CombatAttribute.BlockArmor);

        EnumRegister.Register(lua, "COMBAT_PARAM_CREATEITEM", CombatAttribute.CreateItem);
        EnumRegister.Register(lua, "COMBAT_PARAM_AGGRESSIVE", CombatAttribute.Aggressive);
        EnumRegister.Register(lua, "COMBAT_PARAM_DISPEL", CombatAttribute.Dispel);
        EnumRegister.Register(lua, "COMBAT_PARAM_USECHARGES", CombatAttribute.UseCharges);
        EnumRegister.Register(lua, "COMBAT_PARAM_CASTSOUND", CombatAttribute.CastSound);
        EnumRegister.Register(lua, "COMBAT_PARAM_IMPACTSOUND", CombatAttribute.ImpactSound);
        EnumRegister.Register(lua, "COMBAT_PARAM_CHAIN_EFFECT", CombatAttribute.ChainEffect);
    }

    private static void RegisterDamageEnum(NLua.Lua lua)
    {
        EnumRegister.Register(lua, "COMBAT_NONE", DamageType.None);
        EnumRegister.Register(lua, "COMBAT_PHYSICALDAMAGE", DamageType.Physical);
        EnumRegister.Register(lua, "COMBAT_ENERGYDAMAGE", DamageType.Energy);
        EnumRegister.Register(lua, "COMBAT_EARTHDAMAGE", DamageType.Earth);
        EnumRegister.Register(lua, "COMBAT_FIREDAMAGE", DamageType.Fire);
        EnumRegister.Register(lua, "COMBAT_UNDEFINEDDAMAGE", DamageType.Undefined);
        EnumRegister.Register(lua, "COMBAT_LIFEDRAIN", DamageType.LifeDrain);
        EnumRegister.Register(lua, "COMBAT_MANADRAIN", DamageType.ManaDrain);
        //EnumRegister.Register(lua, "COMBAT_HEALING", DamageType.Healing);
        EnumRegister.Register(lua, "COMBAT_DROWNDAMAGE", DamageType.Drown);
        EnumRegister.Register(lua, "COMBAT_ICEDAMAGE", DamageType.Ice);
        EnumRegister.Register(lua, "COMBAT_HOLYDAMAGE", DamageType.Holy);
        EnumRegister.Register(lua, "COMBAT_DEATHDAMAGE", DamageType.Death);
        //EnumRegister.Register(lua, "COMBAT_AGONYDAMAGE",);
        //EnumRegister.Register(lua, "COMBAT_NEUTRALDAMAGE",);
    }
}