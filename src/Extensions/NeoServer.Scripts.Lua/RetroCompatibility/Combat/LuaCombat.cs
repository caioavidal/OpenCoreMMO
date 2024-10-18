using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NeoServer.Game.Common.Combat.Formula;
using NeoServer.Modules.Combat.Definition;
using NeoServer.Scripts.Lua.Attributes;
using NeoServer.Scripts.Lua.RetroCompatibility.Player;

namespace NeoServer.Scripts.Lua.RetroCompatibility.Combat;

[StructLayout(LayoutKind.Sequential)]
public class LuaCombat
{
    public Dictionary<CombatAttribute, object> Parameters { get; } = new();

    public DamageFormula Formula { get; set; }
    public byte[] Area { get; set; }

    [LuaRegister(Name = "setParameter")]
    public void setParameter(CombatAttribute attribute, object value)
    {
        Parameters.TryAdd(attribute, value);
    }

    public void setFormula(CombatFormula attribute, double minA, double minB, double maxA, double maxB)
    {
        Formula = new DamageFormula
        {
            Type = attribute,
            MinA = minA,
            MinB = minB,
            MaxA = maxA,
            MaxB = maxB
        };
    }

    public void setArea(byte[] area)
    {
        Area = area;
    }

    public void execute(LuaPlayer player, object variant)
    {
        // var attackExecutor = IoC.GetInstance<AttackExecutor>();
        // attackExecutor.Execute(player, player.CurrentTarget.Location, new AttackParameter
        // {
        //     Name = "",
        //     DamageType = Parameters.TryGetValue(CombatAttribute.Damage, out var damageType)
        //         ? (DamageType)damageType
        //         : DamageType.None,
        //     BlockArmor = Parameters.TryGetValue(CombatAttribute.BlockArmor, out var blockArmor) && (bool)blockArmor,
        //     Formula = Formula
        // });
        Console.WriteLine("Executed");
    }

    public void Test()
    {
        Console.WriteLine("Oi lua");
    }

    public static void RegisterCombat(NLua.Lua lua)
    {
        lua["Combat"] = () => new LuaCombat();
        // lua.State.Register("Combat", Combat_new);
        // lua.State.NewMetaTable("Combat");
        //
        // lua.State.PushCFunction(Combat_delete);
        // lua.State.SetField(-2, "__gc");
        // lua.State.PushInteger(-1);
        // lua.State.SetField(-2, "__index");
        // lua.State.PushCFunction(Combat_test);
        // lua.State.SetField(-2, "test");
        // lua.State.Pop(1);
    }

    public static int Combat_new(IntPtr luaState)
    {
        KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(luaState);
        LuaCombat luaCombat = new LuaCombat();

        //lua.CheckNumber(1);
        IntPtr userdata = lua.NewUserData(Marshal.SizeOf(typeof(IntPtr)));

        Marshal.WriteIntPtr(userdata, luaCombat.ToIntPtr());

        //lua.GetMetaTable("Combat");

        //lua.PushLightUserData(combat.ToIntPtr());
        lua.SetMetaTable("Combat");
        return 1;
    }

    public static int Combat_delete(IntPtr luaState)
    {
        KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(luaState);
        LuaCombat luaCombat = (LuaCombat)lua.ToUserData(1).ToGcHandle().Target;
        luaCombat = null;
        return 0;
    }

    public static int Combat_test(IntPtr luaState)
    {
        KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(luaState);
        LuaCombat luaCombat = (LuaCombat)lua.ToUserData(1).ToGcHandle().Target;
        luaCombat.Test();
        return 0;
    }
}

public static class ObjectHandleExtensions
{
    public static IntPtr ToIntPtr(this object target)
    {
        return GCHandle.Alloc(target).ToIntPtr();
    }

    public static GCHandle ToGcHandle(this object target)
    {
        return GCHandle.Alloc(target);
    }

    public static IntPtr ToIntPtr(this GCHandle target)
    {
        return GCHandle.ToIntPtr(target);
    }
}