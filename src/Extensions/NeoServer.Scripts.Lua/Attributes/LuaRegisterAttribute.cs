using System;

namespace NeoServer.Scripts.Lua.Attributes;

public class LuaRegisterAttribute: Attribute
{
    public string Name { get; set; }
}