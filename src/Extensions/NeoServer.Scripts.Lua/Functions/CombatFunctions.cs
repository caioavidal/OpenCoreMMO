using System;
using NeoServer.Game.Combat.Attacks.DamageConditionAttack;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Effects.Magical;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NLua;

namespace NeoServer.Scripts.Lua.Functions;

public static class CombatFunctions
{
    public static void RegisterCombatFunctions(this NLua.Lua lua)
    {
        lua["createCombatArea"] = CallCreateCombatAreaFunction;
        lua["causeDamageCondition"] = CallAddDamageCondition;
    }

    private static Coordinate[] CallCreateCombatAreaFunction(Location location, LuaTable table)
    {
        var rows = table.Values.Count;
        var columns = ((LuaTable)table[1])?.Values?.Count ?? 0;

        if (rows == 0 || columns == 0) return null;

        var area = new byte[table.Values.Count, ((LuaTable)table[1]).Values.Count];

        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                area[row, column] = BitConverter.GetBytes((long)((LuaTable)table[row + 1])[column + 1])[0];
            }
        }

        return AreaEffect.Create(location, area);
    }

    private static void CallAddDamageCondition(IThing aggressor, ICombatActor victim,
        int minDamage, int maxDamage, int damageType, byte damageCount, int interval)
    {
        if (maxDamage <= 0) return;

        var damageTypeEnum = (DamageType)damageType;

        var attack = new DamageConditionAttack(damageTypeEnum,
            new MinMax(minDamage, maxDamage), damageCount, interval);

        attack.CauseDamage(aggressor, victim);
    }
}