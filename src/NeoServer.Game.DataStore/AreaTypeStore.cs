using System;
using System.Collections.Generic;
using System.Reflection;

namespace NeoServer.Game.DataStore
{
    public class AreaTypeStore
    {
        private static Dictionary<string, FieldInfo> areas = new Dictionary<string, FieldInfo>(StringComparer.InvariantCultureIgnoreCase);

        public static void Add(string name, FieldInfo area) => areas.TryAdd(name, area);

        public static byte[,] Get(string name) => areas.TryGetValue(name, out var area) ? (byte[,])area.GetValue(null) : default;
    }
}
