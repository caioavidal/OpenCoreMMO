using System;
using NeoServer.Scripts.Lua.Attributes;

namespace NeoServer.Scripts.Lua.RetroCompatibility;

[LuaRegister]
public class Combat
{
    [LuaRegister(Name = "setParameter")]
    public void SetParameter(Enum attribute, Enum value)
    {
    }

    [LuaRegister(Name = "setFormula")]
    public void SetFormula(Enum attribute, double minA, double minB, double maxA, double maxB)
    {
        
    }
}