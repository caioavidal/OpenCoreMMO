using System;
using NeoServer.Application.Features.Combat.Definition;
using NeoServer.Game.Common.Combat;
using NeoServer.Game.Common.Item;

namespace NeoServer.Scripts.Lua.RetroCompatibility;

public static class EnumSetup
{
    public static void Register(NLua.Lua lua)
    {
        RegisterDamageEnum(lua);
        RegisterCombatAttributeEnum(lua);
        RegisterCombatFormulaEnum(lua);
        RegisterSkullEnum(lua);
        RegisterShootTypeEnum(lua);
        RegisterWeaponTypeEnum(lua);
    }

    private static void RegisterCombatFormulaEnum(NLua.Lua lua)
    {
        Register(lua, "COMBAT_FORMULA_UNDEFINED", CombatFormula.None);
        Register(lua, "COMBAT_FORMULA_LEVELMAGIC", CombatFormula.MagicLevel);
        Register(lua, "COMBAT_FORMULA_SKILL", CombatFormula.Skill);
        Register(lua, "COMBAT_FORMULA_DAMAGE", CombatFormula.Damage);
    }

    private static void RegisterCombatAttributeEnum(NLua.Lua lua)
    {
        Register(lua, "COMBAT_PARAM_TYPE", CombatAttribute.Damage);
        Register(lua, "COMBAT_PARAM_EFFECT", CombatAttribute.Effect);
        Register(lua, "COMBAT_PARAM_DISTANCEEFFECT", CombatAttribute.DistanceEffect);
        Register(lua, "COMBAT_PARAM_BLOCKSHIELD", CombatAttribute.BlockShield);
        Register(lua, "COMBAT_PARAM_BLOCKARMOR", CombatAttribute.BlockArmor);

        Register(lua, "COMBAT_PARAM_CREATEITEM", CombatAttribute.CreateItem);
        Register(lua, "COMBAT_PARAM_AGGRESSIVE", CombatAttribute.Aggressive);
        Register(lua, "COMBAT_PARAM_DISPEL", CombatAttribute.Dispel);
        Register(lua, "COMBAT_PARAM_USECHARGES", CombatAttribute.UseCharges);
        Register(lua, "COMBAT_PARAM_CASTSOUND", CombatAttribute.CastSound);
        Register(lua, "COMBAT_PARAM_IMPACTSOUND", CombatAttribute.ImpactSound);
        Register(lua, "COMBAT_PARAM_CHAIN_EFFECT", CombatAttribute.ChainEffect);
    }

    private static void RegisterDamageEnum(NLua.Lua lua)
    {
        Register(lua, "COMBAT_NONE", DamageType.None);
        Register(lua, "COMBAT_PHYSICALDAMAGE", DamageType.Physical);
        Register(lua, "COMBAT_ENERGYDAMAGE", DamageType.Energy);
        Register(lua, "COMBAT_EARTHDAMAGE", DamageType.Earth);
        Register(lua, "COMBAT_FIREDAMAGE", DamageType.Fire);
        Register(lua, "COMBAT_UNDEFINEDDAMAGE", DamageType.Undefined);
        Register(lua, "COMBAT_LIFEDRAIN", DamageType.LifeDrain);
        Register(lua, "COMBAT_MANADRAIN", DamageType.ManaDrain);
        //EnumRegister.Register(lua, "COMBAT_HEALING", DamageType.Healing);
        Register(lua, "COMBAT_DROWNDAMAGE", DamageType.Drown);
        Register(lua, "COMBAT_ICEDAMAGE", DamageType.Ice);
        Register(lua, "COMBAT_HOLYDAMAGE", DamageType.Holy);
        Register(lua, "COMBAT_DEATHDAMAGE", DamageType.Death);
        //EnumRegister.Register(lua, "COMBAT_AGONYDAMAGE",);
        //EnumRegister.Register(lua, "COMBAT_NEUTRALDAMAGE",);
    }

    private static void RegisterSkullEnum(NLua.Lua lua)
    {
        Register(lua, "SKULL_NONE", Skull.None);
        Register(lua, "SKULL_YELLOW", Skull.Yellow);
        Register(lua, "SKULL_GREEN", Skull.Green);
        Register(lua, "SKULL_WHITE", Skull.White);
        Register(lua, "SKULL_RED", Skull.Red);
        Register(lua, "SKULL_BLACK", Skull.Black);
        Register(lua, "SKULL_ORANGE", Skull.Orange);
    }

    private static void RegisterShootTypeEnum(NLua.Lua lua)
    {
        Register(lua, "AMMO_NONE", ShootType.None);
        Register(lua, "AMMO_BOLT", ShootType.Bolt);
        Register(lua, "AMMO_ARROW", ShootType.Arrow);
        Register(lua, "AMMO_SPEAR", ShootType.Spear);
        Register(lua, "AMMO_THROWINGSTAR", ShootType.ThrowingStar);
        Register(lua, "AMMO_THROWINGKNIFE", ShootType.ThrowingKnife);
        Register(lua, "AMMO_STONE", ShootType.SmallStone);
        Register(lua, "AMMO_SNOWBALL", ShootType.SnowBall);
    }

    private static void RegisterWeaponTypeEnum(NLua.Lua lua)
    {
        Register(lua, "WEAPON_NONE", WeaponType.None);
        Register(lua, "WEAPON_SWORD", WeaponType.Sword);
        Register(lua, "WEAPON_CLUB", WeaponType.Club);
        Register(lua, "WEAPON_AXE", WeaponType.Axe);
        Register(lua, "WEAPON_SHIELD", WeaponType.Shield);
        Register(lua, "WEAPON_DISTANCE", WeaponType.Distance);
        Register(lua, "WEAPON_WAND", WeaponType.Magical);
        Register(lua, "WEAPON_AMMO", WeaponType.Ammunition);
        Register(lua, "WEAPON_MISSILE", WeaponType.Missile);
    }
    private static void Register(NLua.Lua lua, string enumName, Enum @enum) => lua[enumName] = @enum;
}