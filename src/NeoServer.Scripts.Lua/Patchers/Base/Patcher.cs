using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace NeoServer.Scripts.Lua.Patchers.Base;

public abstract class Patcher<T>: IPatcher where T:IPatcher
{
    protected abstract HashSet<Type> Types { get; }
    protected abstract string MethodName { get;}
    protected abstract Type[] Params { get;}
    protected abstract string PrefixMethodName { get; }
    public void Patch()
    {
        if (Types is null) return;
        var allClasses = Types;

        var id = Guid.NewGuid().ToString(); 
        var harmony = new Harmony(id);

        foreach (var type in allClasses)
        {
            var originalMethod = GetOriginalMethod(type);

            if (originalMethod is null) continue;

            originalMethod = originalMethod.DeclaringType != type
                ? GetOriginalMethod(originalMethod.DeclaringType)
                : originalMethod;
            
            if (originalMethod is null) continue;
            
            var patches = Harmony.GetPatchInfo(originalMethod);
            if (patches?.Owners?.Any(x=>x == id) ?? false) return; //patched
            
            var methodPrefix = typeof(T).GetMethod(PrefixMethodName, BindingFlags.Static | BindingFlags.NonPublic);

            if (methodPrefix is null) continue;
            

            harmony.Patch(originalMethod, new HarmonyMethod(methodPrefix));
        }
    }

    private MethodInfo GetOriginalMethod(Type type)
    {
        var originalMethod = type.GetMethod(MethodName,
            types: Params, bindingAttr: BindingFlags.Instance | BindingFlags.Public);
        return originalMethod;
    }
}