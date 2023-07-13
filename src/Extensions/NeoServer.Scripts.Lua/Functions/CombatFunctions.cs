using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common.Effects.Magical;
using NeoServer.Game.Common.Location.Structs;
using NLua;

namespace NeoServer.Scripts.Lua.Functions;

public static class CombatFunctions
{
    public static void RegisterCombatFunctions(this NLua.Lua lua)
    {
        lua["createCombatArea"] = CallCreateCombatAreaFunction;
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
}