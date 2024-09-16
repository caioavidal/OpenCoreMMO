using System;

namespace NeoServer.Scripts.Lua.RetroCompatibility;

public class EnumRegister
{
    public static void Register(NLua.Lua lua, string enumName, Enum @enum)
    {
        lua[enumName] = @enum;
    }
}