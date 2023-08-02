using NeoServer.Scripts.Lua.Enums;

namespace NeoServer.Scripts.Lua.Functions.Enums;

public static class EnumRegister
{
    public static void RegisterEnums(this NLua.Lua lua)
    {
        ConditionTypeEnum.Register(lua);
    }
}