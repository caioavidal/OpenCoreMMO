using NeoServer.Game.Common.Contracts.Items.Types.Body;

namespace NeoServer.Scripts.Lua.EventRegister.Binds;

public class WeaponFunctionBind
{
    public static void Setup()
    {
        IWeapon.AttackFunction = (player, item) => LuaScriptCaller.Call(new ItemKey(item),"attack", player, item);
    }
}