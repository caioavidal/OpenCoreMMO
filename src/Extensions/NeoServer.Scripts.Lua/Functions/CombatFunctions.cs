using System;
using NeoServer.Game.Common.Effects.Magical;
using NeoServer.Game.Common.Location.Structs;
using NLua;

namespace NeoServer.Scripts.Lua.Functions;

public static class CombatFunctions
{
    public static void RegisterCombatFunctions(this NLua.Lua lua)
    {
        lua["createCombatArea"] = CallSetCombatAreaFunction;
        //  lua["causeDamageCondition"] = CallAddDamageCondition;
    }

    private static byte[] CallSetCombatAreaFunction(LuaTable table)
    {
        var rows = table.Values.Count;
        var columns = (table[1] as LuaTable)?.Values?.Count ?? 0;
        const byte breakLine = byte.MaxValue;

        if (rows == 0 || columns == 0) return null;

        var numberOfPoints = rows * columns;
        Span<byte> area = new byte[numberOfPoints + rows];

        var index = 0;
        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                area[index++] = BitConverter.GetBytes((long)((LuaTable)table[row + 1])[column + 1])[0];
            }

            area[index++] = breakLine;
        }

        //var result = AreaEffect.Create(location, area);
        return area.ToArray();
    }

    // private static void CallAddDamageCondition(IThing aggressor, ICombatActor victim,
    //     int minDamage, int maxDamage, int damageType, byte damageCount, int interval)
    // {
    //     if (maxDamage <= 0) return;
    //
    //     var damageTypeEnum = (DamageType)damageType;
    //
    //     var attack = new DamageConditionAttack(damageTypeEnum,
    //         new MinMax(minDamage, maxDamage), damageCount, interval);
    //
    //     attack.CauseDamage(aggressor, victim);
    // }
}