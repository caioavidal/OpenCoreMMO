// using NeoServer.Application.Features.Combat;
// using NeoServer.Game.Combat;
// using NeoServer.Game.Common.Contracts.Creatures;
// using NeoServer.Game.Common.Contracts.Items;
// using NeoServer.Game.Item.Items.Weapons;
//
// namespace NeoServer.Scripts.Lua.EventRegister.Binds;
//
// public class AttackFunctionBind
// {
//     public static void Setup()
//     {
//         AttackParamsModifier.AttackParameterModifierFunction = (aggressor, target, combatAttackParams)
//             => Call("attack", aggressor, target, combatAttackParams);
//     }
//
//     private static bool Call(string eventName,  IItem item, object param1 = null,
//         object param2 = null, object param3 = null,
//         object param4 = null, object param5 = null,
//         object param6 = null, object param7 = null,
//         object param8 = null, object param9 = null,
//         object param10 = null)
//     {
//         if (LuaEventManager.FindItemScriptByServerId(eventName.ToLower()) is { } script)
//             return (bool)(script.Call(item, param1, param2, param3, param4,
//                     param5, param6, param7, param8, param9, param10)?
//                 .FirstOrDefault() ?? true);
//
//         return false; // continue to the original method
//     }   
// }