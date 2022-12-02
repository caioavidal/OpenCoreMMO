using System.Collections.Generic;

namespace NeoServer.Scripts.Lua;

public static class ItemActionMap
{
    private static readonly Dictionary<int, dynamic> Actions = new();
    public static void Register(int typeId, dynamic action) => Actions[typeId] = action;
    public static T Get<T>(ushort typeId) => Actions.GetValueOrDefault(typeId); 
}