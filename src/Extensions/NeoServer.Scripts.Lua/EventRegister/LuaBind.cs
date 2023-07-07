using NeoServer.Scripts.Lua.EventRegister.Binds;

namespace NeoServer.Scripts.Lua.EventRegister;

public static class LuaBind
{
    public static void Setup()
    {
        ItemFunctionBind.Setup();
    }
}