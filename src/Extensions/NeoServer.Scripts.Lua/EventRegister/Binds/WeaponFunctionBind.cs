using System.Linq;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Creatures.Player;
using NeoServer.Game.Items.Items.Weapons;

namespace NeoServer.Scripts.Lua.EventRegister.Binds;

public static class WeaponFunctionBind
{
    public static void Setup()
    {
        Ammo.PrepareAttack = (weapon, player, enemy, combatAttackParams)
            => Call("attack", weapon, player, enemy, combatAttackParams);

        IWeapon.PostAttackFunction = (weapon, player, enemy)
            => Call("postAttack", weapon, player, enemy);
    }

    private static bool Call(string eventName, IItem item, object param1 = null,
        object param2 = null, object param3 = null,
        object param4 = null, object param5 = null,
        object param6 = null, object param7 = null,
        object param8 = null, object param9 = null,
        object param10 = null)
    {
        if (LuaEventManager.FindItemScriptByServerId(item, eventName.ToLower()) is { } script)
            return (bool)(script.Call(item, param1, param2, param3, param4,
                    param5, param6, param7, param8, param9, param10)?
                .FirstOrDefault() ?? true);

        return false; // continue to the original method
    }
}